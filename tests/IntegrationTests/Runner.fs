module IntegrationTests.Runner

open System.IO
open canopy.mobile
open Expecto
open System.Net


[<EntryPoint>]
let main args =
    try
        let debugMode = args |> Array.contains "--debug"
        let app = TestData.findApp debugMode
        try
            TestData.installApp debugMode app

            runTestsWithArgs { defaultConfig with ``parallel`` = false } args Tests.tests
        with e ->
            printfn "Error: %s" e.Message
            -1
    finally
        TestData.uninstallApp()
        quit()