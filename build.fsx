// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r "paket:
nuget FSharp.Core
nuget Fake.Core.ReleaseNotes
nuget Fake.Core.Process
nuget Fake.IO.FileSystem
nuget Fake.BuildServer.TeamFoundation
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.Core.Environment
nuget Fake.Installer.Wix
nuget Newtonsoft.Json
nuget System.ServiceProcess.ServiceController
nuget Fake.Core.Trace
nuget Fake.IO.Zip
nuget Fake.Tools.Git
nuget Microsoft.Web.Administration
nuget Fake.DotNet.Testing.Expecto
//"

#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
#r "netstandard"
#r "Facades/netstandard" // https://github.com/ionide/ionide-vscode-fsharp/issues/839#issuecomment-396296095
#endif

open System.IO
open Fake.Core
open Fake.DotNet
open Fake.Core.TargetOperators
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

// Read additional information from the release notes document
let releaseNotes = File.ReadAllLines "RELEASE_NOTES.md"

let releaseNotesData =
    releaseNotes
    |> parseAllReleaseNotes

let outDir = "./out"
let deployDir = "./deploy"
let release = List.head releaseNotesData
let packageVersion = SemVerHelper.parse release.NugetVersion


let testExecutables = "tests/**/bin/Debug/**/*Tests*.exe"


let run' timeout (cmd:Lazy<string>) args dir =
    if execProcess (fun info ->
        info.FileName <- cmd.Force()
        if not (String.IsNullOrWhiteSpace dir) then
            info.WorkingDirectory <- dir
        info.Arguments <- args
    ) timeout |> not then
        failwithf "Error while running '%s' with args: %s" (cmd.Force()) args

let run = run' System.TimeSpan.MaxValue


let platformTool tool winTool = lazy(
    let tool = if isUnix then tool else winTool
    tool
    |> ProcessHelper.tryFindFileOnPath
    |> function Some t -> t | _ -> failwithf "%s not found" tool)

let nodeTool = platformTool "node" "node.exe"
let yarnTool = platformTool "yarn" "yarn.cmd"

let srcDir = __SOURCE_DIRECTORY__ </> "src"

let testDir = __SOURCE_DIRECTORY__ </> "tests" </> "IntegrationTests"

let dotnetcliVersion = DotNetCli.GetDotNetSDKVersionFromGlobalJson()

let mutable dotnetExePath = "dotnet"

let runDotnet workingDir args =
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue
    if result <> 0 then failwithf "dotnet %s failed" args

let gradleTool = platformTool "android/gradlew" ("android" </> "gradlew.bat" |> FullName)
let reactNativeTool = platformTool "react-native" "react-native.cmd"

let androidSDKPath =
    match environVarOrNone "ANDROID_HOME" with
    | Some path -> path
    | None ->
        if isWindows then
            let p1 = ProgramFilesX86 </> "Android" </> "android-sdk"
            if Directory.Exists p1 then FullName p1 else
            let p2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) </> "Android" </> "sdk"
            if Directory.Exists p2 then FullName p2 else
            failwithf "Can't find Android SDK in %s or %s" p1 p2
        else
            let p3 = Environment.GetFolderPath(Environment.SpecialFolder.Personal) </> "Library/Android/sdk"
            if Directory.Exists p3 then FullName p3 else
            failwithf "Can't find Android SDK in %s, please set ANDROID_HOME enviromental variable" p3

let adbTool = platformTool (androidSDKPath </> "platform-tools/adb") (androidSDKPath </> "platform-tools/adb.exe")


setEnvironVar "ANDROID_HOME" androidSDKPath
setEnvironVar "ANDROID_SDK_ROOT" androidSDKPath

if buildServer = BuildServer.GitLabCI then
    let newPATH = environVar "PATH" + @";C:\Program Files (x86)\Microsoft F#\v4.0"
    setEnvironVar "PATH" newPATH

let killDotnetCli() =
    killProcess "dotnet"
    killProcess "dotnet.exe"

let killADB() =
    killProcess "adb"
    killProcess "adb.exe"


if isLocalBuild then () else killDotnetCli()

Target "Clean" (fun _ ->
    CleanDir outDir
    CleanDir deployDir
    ProcessHelper.killProcess "node"
    !! "./**/AppiumLog.txt" |> Seq.iter File.Delete
    CleanDir "./android/build"
    CleanDir "./android/app/build"
)

FinalTarget "CloseAndroid" (fun _ ->
    if isLocalBuild then () else
    killADB()
    killDotnetCli()
    killProcess "qemu-system-i386"
    killProcess "qemu-system-aarch64"
    killProcess "qemu-system-armel"
    killProcess "qemu-system-mips64el"
    killProcess "qemu-system-mipsel"
    killProcess "qemu-system-x86_64"
    killProcess "emulator-crash-service"
    killProcess "emulator64-crash-service"
    killProcess "node"
)

Target "InstallDotNetCore" (fun _ ->
    dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion

    let fi = FileInfo dotnetExePath
    let SEPARATOR = if isWindows then ";" else ":"
    Environment.SetEnvironmentVariable(
        "PATH",
        fi.Directory.FullName + SEPARATOR + System.Environment.GetEnvironmentVariable "PATH",
        EnvironmentVariableTarget.Process)
)

Target "Restore" (fun _ ->
    setEnvironVar "ANDROID_HOME" androidSDKPath

    printfn "Node version:"
    run nodeTool "--version" __SOURCE_DIRECTORY__
    printfn "Yarn version:"
    run yarnTool "--version" __SOURCE_DIRECTORY__
    run yarnTool "install --frozen-lockfile" __SOURCE_DIRECTORY__
    runDotnet srcDir "restore"
    runDotnet testDir "restore"
)

Target "BuildTests" (fun _ ->
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- testDir
            info.Arguments <- "build") TimeSpan.MaxValue
    if result <> 0 then failwith "dotnet build issue."
)

Target "RunTests" (fun _ ->
    ActivateFinalTarget "CloseAndroid"

    !! testExecutables
    |> Expecto (fun p -> { p with Parallel = false } )
    |> ignore
)

let gradleFile = "./android/app/build.gradle"

let getCurrentAndroidVersionCode() =
    File.ReadAllLines gradleFile
    |> Seq.tryPick (fun line ->
        if line.TrimStart().StartsWith("versionCode ") then
            Some(line.Replace("versionCode","").Trim() |> int)
        else None)
    |> fun v -> defaultArg v 1

Target "SetVersionAndroid" (fun _ ->
    let lines =
        File.ReadAllLines gradleFile
        |> Seq.map (fun line ->
            if line.TrimStart().StartsWith("versionCode ") then
                let currentVersionCode = line.Replace("versionCode","").Trim() |> int
                let indent = line.Substring(0,line.IndexOf("versionCode"))
                sprintf "%sversionCode %d" indent (currentVersionCode + 1)
            elif line.TrimStart().StartsWith("versionName ") then
                let indent = line.Substring(0,line.IndexOf("versionName"))
                sprintf "%sversionName \"%O\"" indent release.NugetVersion
            else line)
    File.WriteAllLines(gradleFile,lines)

    let fileName = "./package.json"
    let lines =
        File.ReadAllLines fileName
        |> Seq.map (fun line ->
            if line.TrimStart().StartsWith("\"version\":") then
                let indent = line.Substring(0,line.IndexOf("\""))
                sprintf "%s\"version\": \"%O\"," indent release.NugetVersion
            else line)
    File.WriteAllLines(fileName,lines)
)

Target "SetReleaseNotes" (fun _ ->
    let lines = [
            "module internal ReleaseNotes"
            ""
            (sprintf "let AppVersion = \"%s\"" release.NugetVersion)
            ""
            (sprintf "let IsPrerelease = %b" (release.SemVer.PreRelease <> None))
            ""
            (sprintf "let AndroidVersionCode = %d" (getCurrentAndroidVersionCode()))
            ""
            "let Notes = \"\"\""] @ Array.toList releaseNotes @ ["\"\"\""]
    File.WriteAllLines("src/ReleaseNotes.fs",lines)
)

Target "PrepareRelease" (fun _ ->
    Git.Branches.checkout "" false "master"
    Git.CommandHelper.directRunGitCommand "" "fetch origin" |> ignore
    Git.CommandHelper.directRunGitCommand "" "fetch origin --tags" |> ignore

    StageAll ""
    Git.Commit.Commit "" (sprintf "Release %O" release.NugetVersion)
    Git.Branches.pushBranch "" "origin" "master"

    let tagName = string release.NugetVersion
    Git.Branches.tag "" tagName
    Git.Branches.pushTag "" "origin" tagName
)


Target "CompileForTest" (fun _ ->
    ActivateFinalTarget "KillProcess"

    run yarnTool "run fable-splitter -c splitter.config.js --define TEST" srcDir
)

Target "AssembleForTest" (fun _ ->
    run gradleTool "assembleRelease --console plain" "android"
)


type NativeApps =
| Android
| IOs

let nativeApp =
    if hasBuildParam "ios" then IOs
    elif hasBuildParam "android" then Android
    elif isMacOS then
        IOs
    else
        Android

let reactiveCmd =
    match nativeApp with
    | Android -> "react-native run-android"
    | IOs -> "react-native run-ios"

Target "BuildRelease" (fun _ ->
    ActivateFinalTarget "KillProcess"

    run yarnTool "run fable-splitter -c splitter.config.js --define RELEASE" srcDir

    run gradleTool "assembleRelease --console plain" "android"

    let outFile = "android" </> "app" </> "build" </> "outputs" </> "apk"  </> "release" </> "app-release.apk"

    Copy deployDir [outFile]
    let fi = FileInfo (deployDir </> "app-release.apk")
    fi.MoveTo (deployDir </> sprintf "Nightwatch.%s.apk" release.NugetVersion)
)

Target "Debug" (fun _ ->
    run yarnTool "run fable-splitter -c splitter.config.js --define DEBUG" srcDir

    let fablewatch = async { run yarnTool "run fable-splitter -c splitter.config.js -w --define DEBUG" srcDir }

    let reactNativeTool = async { run yarnTool reactiveCmd "" }

    Async.Parallel [| fablewatch; reactNativeTool |]
    |> Async.RunSynchronously
    |> ignore
)

FinalTarget "KillProcess" (fun _ ->
    killDotnetCli()
    killADB()
)

Target "Deploy" (fun _ ->
    () // TODO:
)

Target "Default" DoNothing

"Clean"
==> "SetReleaseNotes"
==> "SetVersionAndroid"
==> "InstallDotNetCore"
==> "Restore"
==> "CompileForTest"
==> "AssembleForTest"
==> "BuildTests"
==> "RunTests"
==> "Default"
==> "BuildRelease"
==> "Deploy"

"Restore"
==> "Debug"

"SetVersionAndroid"
==> "PrepareRelease"

RunTargetOrDefault "Default"
