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
open Fake.DotNet.Testing
// Read additional information from the release notes document
let releaseNotes = File.ReadAllLines "RELEASE_NOTES.md"

let release = ReleaseNotes.load "RELEASE_NOTES.md"

let outDir = "./out"
let deployDir = "./deploy"
let serverTestsPath = Path.getFullName "./tests/Server"
let sharedTestsPath = Path.getFullName "./tests/Shared"

// --------------------------------------------------------------------------------------
// PlatformTools
// --------------------------------------------------------------------------------------

let platformTool tool winTool =
    let tool =
        if Environment.isUnix then tool else winTool

    match ProcessUtils.tryFindFileOnPath tool with
    | Some t -> t
    | _ ->
        let errorMsg =
            tool
            + " was not found in path. "
            + "Please install it and make sure it's available from your path. "
            + "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"

        failwith errorMsg

let nodeTool = platformTool "node" "node.exe"
// let yarnTool = platformTool "yarn" "yarn.cmd"
let npmTool = platformTool "npm" "npm.cmd"

let dotnet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""

    if result.ExitCode <> 0
    then failwithf "'dotnet %s' failed in %s" cmd workingDir

let npm args workingDir =

    let npmPath =
        match ProcessUtils.tryFindFileOnPath "npm" with
        | Some path -> path
        | None ->
            "npm was not found in path. Please install it and make sure it's available from your path. "
            + "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
            |> failwith

    let arguments =
        args |> String.split ' ' |> Arguments.OfArgs

    RawCommand(npmPath, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let runTool cmd args workingDir =
    let arguments =
        args |> String.split ' ' |> Arguments.OfArgs

    RawCommand(cmd, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore
let srcDir = __SOURCE_DIRECTORY__ </> "src"

let testDir = __SOURCE_DIRECTORY__ </> "tests" </> "IntegrationTests"


let gradleTool = platformTool "android/gradlew" ("android" </> "gradlew.bat" |> Path.getFullName)
let reactNativeTool = platformTool "react-native" "react-native.cmd"

let androidSDKPath =
    match Environment.environVarOrNone "ANDROID_HOME" with
    | Some path -> path
    | None -> ""
        // if Environment.isWindows then
        //     let p1 = Environment.ProgramFilesX86 </> "Android" </> "android-sdk"
        //     if Directory.Exists p1 then Path.getFullName p1 else
        //     let p2 = Environment.GetFolderPath(Environment.SpecialFolder.) </> "Android" </> "sdk"
        //     if Directory.Exists p2 then FullName p2 else
        //     failwithf "Can't find Android SDK in %s or %s" p1 p2
        // else
        //     let p3 = Environment.GetFolderPath(Environment.SpecialFolder.Personal) </> "Library/Android/sdk"
        //     if Directory.Exists p3 then FullName p3 else
        //     failwithf "Can't find Android SDK in %s, please set ANDROID_HOME enviromental variable" p3

// let adbTool = platformTool (androidSDKPath </> "platform-tools/adb") (androidSDKPath </> "platform-tools/adb.exe")


Environment.setEnvironVar "ANDROID_HOME" androidSDKPath
Environment.setEnvironVar "ANDROID_SDK_ROOT" androidSDKPath

if BuildServer.buildServer = GitLabCI then
    let newPATH = Environment.environVar "PATH" + @";C:\Program Files (x86)\Microsoft F#\v4.0"
    Environment.setEnvironVar "PATH" newPATH

let killDotnetCli() =
    Process.killAllByName "dotnet"
    Process.killAllByName "dotnet.exe"

let killADB() =
    Process.killAllByName "adb"
    Process.killAllByName "adb.exe"


if BuildServer.isLocalBuild then () else killDotnetCli()

Target.create "Clean" (fun _ ->
    Shell.cleanDir outDir
    Shell.cleanDir deployDir
    Process.killAllByName "node"
    !! "./**/AppiumLog.txt" |> Seq.iter File.Delete
    Shell.cleanDir "./android/build"
    Shell.cleanDir "./android/app/build"
)

Target.createFinal "CloseAndroid" (fun _ ->
    if BuildServer.isLocalBuild then () else
    killADB()
    killDotnetCli()
    Process.killAllByName "qemu-system-i386"
    Process.killAllByName "qemu-system-aarch64"
    Process.killAllByName "qemu-system-armel"
    Process.killAllByName "qemu-system-mips64el"
    Process.killAllByName "qemu-system-mipsel"
    Process.killAllByName "qemu-system-x86_64"
    Process.killAllByName "emulator-crash-service"
    Process.killAllByName "emulator64-crash-service"
    Process.killAllByName "node"
)

Target.create "Restore" (fun _ ->
    Environment.setEnvironVar "ANDROID_HOME" androidSDKPath

    printfn "Node version:"
    runTool nodeTool "--version" __SOURCE_DIRECTORY__
    printfn "Npm version:"
    runTool npmTool "--version" __SOURCE_DIRECTORY__
    runTool npmTool "install" __SOURCE_DIRECTORY__
    dotnet "restore" srcDir
    // dotnet "restore" testDir
)

Target.create "BuildTests" (fun _ ->
    dotnet "build" testDir
)

// Target.create "ExecuteTests" (fun _ ->
//     Environment.setEnvironVar "status" "Development"
//     dotnet "build" sharedTestsPath
//     [ async { dotnet "run" serverTestsPath }
//       async { npm "run test:build" "." } ]
//     |> Async.Parallel
//     |> Async.RunSynchronously
//     |> ignore
// )


let gradleFile = "./android/app/build.gradle"

let getCurrentAndroidVersionCode() =
    File.ReadAllLines gradleFile
    |> Seq.tryPick (fun line ->
        if line.TrimStart().StartsWith("versionCode ") then
            Some(line.Replace("versionCode","").Trim() |> int)
        else None)
    |> fun v -> defaultArg v 1

Target.create "SetVersionAndroid" (fun _ ->
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

Target.create "SetReleaseNotes" (fun _ ->
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

Target.create "PrepareRelease" (fun _ ->
    Fake.Tools.Git.Branches.checkout "" false "master"
    Fake.Tools.Git.CommandHelper.directRunGitCommand "" "fetch origin"
    |> ignore
    Fake.Tools.Git.CommandHelper.directRunGitCommand "" "fetch origin --tags"
    |> ignore

    Fake.Tools.Git.Staging.stageAll ""
    Fake.Tools.Git.Commit.exec "" (sprintf "Bumping version to %O" release.NugetVersion)
    Fake.Tools.Git.Branches.pushBranch "" "origin" "master"

    let tagName = string release.NugetVersion
    Fake.Tools.Git.Branches.tag "" tagName
    Fake.Tools.Git.Branches.pushTag "" "origin" tagName
)


Target.create "CompileForTest" (fun _ ->
    Target.activateFinal "KillProcess"
    DotNet.exec id "fable" "watch src/ --outDir ./output/Nightwatch --define TEST"
            |> ignore
)

Target.create "AssembleForTest" (fun _ ->
    runTool gradleTool "assembleRelease --console plain" "android"
)


type NativeApps =
| Android
| IOs

let nativeApp =
    if Environment.hasEnvironVar "ios" then IOs
    elif Environment.hasEnvironVar "android" then Android
    elif Environment.isMacOS then
        IOs
    else
        Android

let reactiveCmd =
    match nativeApp with
    | Android -> "react-native run-android"
    | IOs -> "react-native run-ios"

Target.create "BuildRelease" (fun _ ->
    Target.activateFinal "KillProcess"

    dotnet "fable run fable-splitter -c splitter.config.js --define RELEASE" srcDir

    runTool gradleTool "assembleRelease --console plain" "android"

    let outFile = "android" </> "app" </> "build" </> "outputs" </> "apk"  </> "release" </> "app-release.apk"

    Shell.copy deployDir [outFile]
    let fi = FileInfo (deployDir </> "app-release.apk")
    fi.MoveTo (deployDir </> sprintf "Nightwatch.%s.apk" release.NugetVersion)
)

Target.create "Debug" (fun _ ->
    dotnet "run fable-splitter -c splitter.config.js --define DEBUG" srcDir

    let fablewatch = async { dotnet "run fable-splitter -c splitter.config.js -w --define DEBUG" srcDir }

    let reactNativeTool = async { runTool npmTool reactiveCmd "" }

    Async.Parallel [| fablewatch; reactNativeTool |]
    |> Async.RunSynchronously
    |> ignore
)

Target.createFinal "KillProcess" (fun _ ->
    killDotnetCli()
    killADB()
)

Target.create "Deploy" (fun _ ->
    () // TODO:
)

Target.create "Default" ignore

"Clean"
==> "SetReleaseNotes"
==> "SetVersionAndroid"
==> "Restore"
==> "CompileForTest"
==> "AssembleForTest"
// ==> "ExecuteTests"
==> "Default"
==> "BuildRelease"
==> "Deploy"

"Restore"
==> "Debug"

"SetVersionAndroid"
==> "PrepareRelease"

Target.runOrDefaultWithArguments "Default"
