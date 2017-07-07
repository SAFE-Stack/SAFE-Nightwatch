// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r @"packages/build/FAKE/tools/FakeLib.dll"

open System
open System.Diagnostics
open System.IO
open Fake
open Fake.Git
open Fake.ProcessHelper
open Fake.ReleaseNotesHelper
open Fake.ZipHelper

// Read additional information from the release notes document
let releaseNotes = File.ReadAllLines "RELEASE_NOTES.md"

let releaseNotesData =
    releaseNotes
    |> parseAllReleaseNotes

let deployDir = "./deploy"

let release = List.head releaseNotesData

let msg =  release.Notes |> List.fold (fun r s -> r + s + "\n") ""
let releaseMsg = (sprintf "Release %s\n" release.NugetVersion) + msg

let run' timeout cmd args dir =
    if execProcess (fun info ->
        info.FileName <- cmd
        if not (String.IsNullOrWhiteSpace dir) then
            info.WorkingDirectory <- dir
        info.Arguments <- args
    ) timeout |> not then
        failwithf "Error while running '%s' with args: %s" cmd args

let run = run' System.TimeSpan.MaxValue


let platformTool tool winTool =
    let tool = if isUnix then tool else winTool
    tool
    |> ProcessHelper.tryFindFileOnPath
    |> function Some t -> t | _ -> failwithf "%s not found" tool

let nodeTool = platformTool "node" "node.exe"
let yarnTool = platformTool "yarn" "yarn.cmd"

let srcDir = __SOURCE_DIRECTORY__ </> "src"

let dotnetcliVersion = "1.0.4"

let mutable dotnetExePath = "dotnet"

let runDotnet workingDir args =
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue
    if result <> 0 then failwithf "dotnet %s failed" args

let gradleTool = platformTool "android/gradlew" ("android" </> "gradlew.bat" |> FullName)
let reactNativeTool() = platformTool "react-native" "react-native.cmd"

let scpTool = @"C:\Program Files (x86)\Git\usr\bin\scp.exe"
let sshTool = @"C:\Program Files (x86)\Git\usr\bin\ssh.exe"

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

killProcess "dotnet"
killProcess "dotnet.exe"

Target "Clean" (fun _ ->
    CleanDir deployDir
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
    run yarnTool "install" __SOURCE_DIRECTORY__
    runDotnet srcDir "restore"
)

Target "SetVersionAndroid" (fun _ ->
    let fileName = "./android/app/build.gradle"
    let v = SemVerHelper.parse release.NugetVersion
    let lines =
        File.ReadAllLines fileName
        |> Seq.map (fun line ->
            if line.TrimStart().StartsWith("versionCode ") then
                let currentVersionCode = line.Replace("versionCode","").Trim() |> int
                let indent = line.Substring(0,line.IndexOf("versionCode"))
                sprintf "%sversionCode %d" indent (currentVersionCode + 1)
            elif line.TrimStart().StartsWith("versionName ") then
                let indent = line.Substring(0,line.IndexOf("versionName"))
                sprintf "%sversionName \"%O\"" indent release.NugetVersion
            else line)
    File.WriteAllLines(fileName,lines)
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

Target "Build" (fun _ ->
    ActivateFinalTarget "KillProcess"
    if buildServer = BuildServer.GitLabCI then
        let newPATH = environVar "PATH" + @";C:\Program Files (x86)\Microsoft F#\v4.0"
        setEnvironVar "PATH" newPATH

    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- srcDir
            info.Arguments <- " fable npm-run build") TimeSpan.MaxValue
    if result <> 0 then failwith "fable shut down. Please check logs above"
    run gradleTool "assembleRelease" "android"

    let outFile = "android" </> "app" </> "build" </> "outputs" </> "apk" </> "app-release-unsigned.apk"
    Copy deployDir [outFile]
    let fi = FileInfo (deployDir </> "app-release-unsigned.apk")
    fi.MoveTo (deployDir </> sprintf "Nightwatch.%s.apk" release.NugetVersion)
)

Target "Debug" (fun _ ->
    let dotnetwatch = async {
        let result =
            ExecProcess (fun info ->
                info.FileName <- dotnetExePath
                info.WorkingDirectory <- srcDir
                info.Arguments <- " fable npm-run start") TimeSpan.MaxValue
        if result <> 0 then failwith "fable shut down." }

    let reactNativeTool = async { run (reactNativeTool()) "run-android" "" }
    let openBrowser = async {
        System.Threading.Thread.Sleep(5000)
        Process.Start("chrome.exe","http://localhost:8081/debugger-ui") |> ignore }

    Async.Parallel [| dotnetwatch; reactNativeTool; openBrowser |]
    |> Async.RunSynchronously
    |> ignore
)

FinalTarget "KillProcess" (fun _ ->
    killProcess "dotnet"
    killProcess "dotnet.exe"
)

Target "Deploy" (fun _ ->
    () // TODO:
)

Target "Default" DoNothing

"Clean"
==> "InstallDotNetCore"
==> "Restore"
==> "SetVersion"
==> "SetVersionAndroid"
==> "Build"
==> "Default"
==> "Deploy"

"SetVersion"
==> "Debug"


RunTargetOrDefault "Default"
