module IntegrationTests.ExpectoHelper

open canopy.mobile
open Expecto
open System.IO

let screenShotDir = "../../../../../temp/screenshots"

let testCase name fn =
  testCase 
    name
    (fun () -> 
        try 
            fn ()
        with 
        | _ ->
            let fileName = name + ".png"
            let p = Path.Combine(Path.GetFullPath screenShotDir,fileName)
            printfn "Taking screenshot to %s" p
            screenshot screenShotDir fileName
            reraise())
