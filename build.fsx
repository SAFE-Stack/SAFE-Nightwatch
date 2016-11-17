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

let run cmd args dir =
    if execProcess( fun info ->
        info.FileName <- cmd
        if not( String.IsNullOrWhiteSpace dir) then
            info.WorkingDirectory <- dir
        info.Arguments <- args
    ) System.TimeSpan.MaxValue |> not then
        failwithf "Error while running '%s' with args: %s" cmd args

let platformTool tool path =
    isUnix |> function | true -> tool | _ -> path

let nodePath = @"C:\Program Files\nodejs\node.exe" |> FullName

let npmTool = platformTool "npm" (@"C:\Program Files\nodejs\npm.cmd" |> FullName)

let fableTool = platformTool "fable" ("node_modules" </> ".bin" </> "fable.cmd" |> FullName)

let gradleTool = platformTool "android/gradlew" ("android" </> "gradlew.bat" |> FullName)
let reactNativeTool = platformTool "react-native" ("node_modules" </> ".bin" </> "react-native.cmd" |> FullName)

let scpTool = @"C:\Program Files (x86)\Git\usr\bin\scp.exe"
let sshTool = @"C:\Program Files (x86)\Git\usr\bin\ssh.exe"

let androidSDKPath =
    let p1 = ProgramFilesX86 </> "Android" </> "android-sdk"
    if Directory.Exists p1 then FullName p1 else
    let p2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) </> "Android" </> "sdk"
    if Directory.Exists p2 then FullName p2 else
    failwithf "Can't find Android SDK in %s or %s" p1 p2

Target "Clean" (fun _ ->
    CleanDir deployDir
)

Target "NPMInstall" (fun _ ->
    setEnvironVar "ANDROID_HOME" androidSDKPath

    run npmTool "install" ""
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
    if buildServer = BuildServer.GitLabCI then
        let newPATH = environVar "PATH" + @";C:\Program Files (x86)\Microsoft F#\v4.0"
        setEnvironVar "PATH" newPATH

    run fableTool "--verbose --symbols PRODUCTION" ""
    run gradleTool "assembleRelease" "android"
    
    let outFile = "android" </> "app" </> "build" </> "outputs" </> "apk" </> "app-release.apk"
    Copy deployDir [outFile]
    let fi = FileInfo (deployDir </> "app-release.apk")
    fi.MoveTo (deployDir </> sprintf "msu.Reading.%s.apk" release.NugetVersion)
)

Target "Debug" (fun _ ->
    run fableTool "" ""
    run reactNativeTool "run-android" ""    
    Process.Start("chrome.exe","http://localhost:8081/debugger-ui") |> ignore
    run fableTool "--watch" ""
)

Target "Deploy" (fun _ ->
    () // TODO:
)

Target "Default" DoNothing

"Clean"
==> "NPMInstall"
==> "SetVersion"
==> "SetVersionAndroid"
==> "Build"
==> "Default"
==> "Deploy"

"SetVersion"
==> "Debug"


RunTargetOrDefault "Default"
