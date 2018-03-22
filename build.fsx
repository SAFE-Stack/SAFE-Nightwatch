// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r @"packages/build/FAKE/tools/FakeLib.dll"

open System
open System.Diagnostics
open System.IO
open System.Text.RegularExpressions
open Fake
open Fake.Git
open Fake.ProcessHelper
open Fake.ReleaseNotesHelper
open Fake.ZipHelper
open Fake.Testing.Expecto

// Read additional information from the release notes document
let releaseNotes = File.ReadAllLines "RELEASE_NOTES.md"

let releaseNotesData =
    releaseNotes
    |> parseAllReleaseNotes

let outDir = "./out"
let deployDir = "./deploy"
let nodeModulesBinDir = "./node_modules/.bin"

let release = List.head releaseNotesData
let packageVersion = SemVerHelper.parse release.NugetVersion


// Pattern specifying assemblies to be tested using expecto
let testExecutables = "tests/**/bin/Debug/**/*Tests*.exe"


// Default target configuration
let configuration = "Release"

let msg =  release.Notes |> List.fold (fun r s -> r + s + "\n") ""
let releaseMsg = (sprintf "Release %s\n" release.NugetVersion) + msg

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
    |> List.tryPick ProcessHelper.tryFindFileOnPath
    |> function Some t -> t | _ -> failwithf "%A not found" tool)

let nodeTool = platformTool ["node"] ["node.exe"]
let yarnTool = platformTool ["yarn"] ["yarn.cmd"]

let srcDir = __SOURCE_DIRECTORY__ </> "src"

let testDir = __SOURCE_DIRECTORY__ </> "tests" </> "IntegrationTests"

let dotnetcliVersion = "2.0.0"

let mutable dotnetExePath = "dotnet"

let runDotnet workingDir args =
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue
    if result <> 0 then failwithf "dotnet %s failed" args

let gradleTool = platformTool ["android/gradlew"] ["android" </> "gradlew.bat" |> FullName]
let reactNativeTool = platformTool [nodeModulesBinDir </> "react-native"] [nodeModulesBinDir </> "react-native.cmd"]

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

let adbTool = platformTool [androidSDKPath </> "platform-tools/adb"] [androidSDKPath </> "platform-tools/adb.exe"]

let scpTool = platformTool ["scp"] [@"C:\Program Files\Git\usr\bin\scp.exe"; @"C:\Program Files (x86)\Git\usr\bin\scp.exe"]
let sshTool = platformTool ["ssh"] [@"C:\Program Files\Git\usr\bin\ssh.exe"; @"C:\Program Files (x86)\Git\usr\bin\ssh.exe"]


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


killDotnetCli()

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
)

Target "SetVersion" (fun _ ->
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

    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- srcDir
            info.Arguments <- " fable npm-run compile-for-test") TimeSpan.MaxValue

    if result <> 0 then failwith "fable shut down. Please check logs above"
)

Target "AssembleForTest" (fun _ ->
    run gradleTool "assembleRelease --console plain" "android"
)

Target "BuildRelease" (fun _ ->
    ActivateFinalTarget "KillProcess"

    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- srcDir
            info.Arguments <- " fable npm-run build") TimeSpan.MaxValue
    if result <> 0 then failwith "fable shut down. Please check logs above"
    run gradleTool "assembleRelease --console plain" "android"

    let outFile = "android" </> "app" </> "build" </> "outputs" </> "apk" </> "app-release.apk"
    Copy deployDir [outFile]
    let fi = FileInfo (deployDir </> "app-release.apk")
    fi.MoveTo (deployDir </> sprintf "Nightwatch.%s.apk" release.NugetVersion)
)

Target "Debug" (fun _ ->
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- srcDir
            info.Arguments <- " fable npm-run cold-start") TimeSpan.MaxValue
    if result <> 0 then failwith "fable shut down."
    
    let dotnetwatch = async {
        let result =
            ExecProcess (fun info ->
                info.FileName <- dotnetExePath
                info.WorkingDirectory <- srcDir
                info.Arguments <- " fable npm-run start") TimeSpan.MaxValue
        if result <> 0 then failwith "fable shut down." }

    let reactNativeTool = async {  run reactNativeTool "run-android" "" }

    Async.Parallel [| dotnetwatch; reactNativeTool |]
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
==> "InstallDotNetCore"
==> "Restore"
==> "SetReleaseNotes"
==> "SetVersionAndroid"
==> "SetVersion"
==> "CompileForTest"
==> "AssembleForTest"
==> "BuildTests"
==> "RunTests"
==> "Default"
==> "BuildRelease"
==> "Deploy"

"SetVersion"
==> "Debug"

"SetVersionAndroid"
==> "PrepareRelease"

RunTargetOrDefault "Default"
