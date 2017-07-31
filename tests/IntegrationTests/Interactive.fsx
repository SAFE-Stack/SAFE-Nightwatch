#load @"../../.paket/load/net462/Test/test.group.fsx"
#load @"ServiceData.fs"
#load @"TestData.fs"

open System
open System.IO
open canopy.mobile
open IntegrationTests

let init() =
    let debugMode = true
    let app = TestData.findApp debugMode
    try
        TestData.uninstallApp()
    with
    | _ -> ()

    try
        TestData.installApp debugMode app
    with e ->
        printfn "Error: %s" e.Message

let exit() =
    TestData.uninstallApp()
    quit()

init()


exit()
