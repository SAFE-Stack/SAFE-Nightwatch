module IntegrationTests.SoundCheck

open System
open System.IO
open Expecto
open canopy.mobile
open IntegrationTests.ExpectoHelper

let tests =
    testList "sound check" [
        testCase "can take screenshot" (fun () ->
            waitFor "tv:Nightwatch"
            let filename = DateTime.Now.ToString("MMM-d_HH-mm-ss-fff")
            screenshot screenShotDir filename
            Expect.isTrue (File.Exists(Path.Combine(screenShotDir,filename + ".png"))) "Screenshot exists"
        )
    ]