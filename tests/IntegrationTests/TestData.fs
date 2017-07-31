module IntegrationTests.TestData

open System.IO
open canopy.mobile
open Expecto
open System.Net

let startOnEmulator app =
    let settings = 
        { DefaultAndroidSettings with 
            AVDName = "AVD_for_Nexus_6_by_Google"
            Silent = true }
    start settings app

let findApp debugMode =
    let app = 
        let fileName = "app-release.apk"
        if debugMode then
            Path.Combine("." , "android", "app", "build", "outputs", "apk", fileName)
            |> Path.GetFullPath
        else
           Path.Combine("..", "..",  "..",  "..", "..", "android", "app", "build", "outputs", "apk", fileName)
           |> Path.GetFullPath
   
    if not (File.Exists app) then
        failwithf "App %s doesn't exist." app

    app

let private installedApp = ref None

let installApp debugMode app =
    if not debugMode then
        startOnEmulator app
    else
        if isAndroidEmulatorRunning() then
            startOnEmulator app 
        else
            startOnDevice app

    installedApp := Some app


let uninstallApp() =
    match !installedApp with
    | Some app -> driver.RemoveApp app
    | _ -> ()