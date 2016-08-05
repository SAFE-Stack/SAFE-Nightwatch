module DemoData

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore

let createDemoData() =
    async { 
        try
            let! count = DB.count<Model.LocationCheckRequest>()
            if count = 0 then
                // Create demo data 
                do! DB.addMultiple<Model.LocationCheckRequest>(
                        [| { LocationId = "X1"; Name = "Bell tower"; Address = "Market place" } 
                           { LocationId = "X2"; Name = "Graveyard"; Address = "Right next to church" } |])
        with
        | error -> ()
    } 
    |> Async.StartImmediate