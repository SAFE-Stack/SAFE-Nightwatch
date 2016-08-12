module Database

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore
open Fable.Import.Fetch
open Fable.Helpers.Fetch
open Model

let getRequestsAndMatchingResults() =
    async { 
        let! requests = DB.getAll<LocationCheckRequest>()
        let! results = DB.getAll<LocationCheckResult>()

        return
            requests
            |> Array.map (fun request -> request,results |> Array.tryFind (fun result -> result.LocationId = request.LocationId))
    }

let createDemoData() =
    async { 
        try
            let! count = DB.count<LocationCheckRequest>()
                    
            do! DB.add 
                    { LocationId = "X0"
                      Name = "Hall"
                      Address = "Inside the main building" }

            if count = 0 then
                // Fetch demo data 
                let! requests =
                    fetchAs<LocationCheckRequest[]>
                      ("https://raw.githubusercontent.com/fsprojects/fable-react_native-demo/master/demodata/LocationCheckRequests.json",
                      [])
                do! DB.addMultiple requests
        with
        | error -> 
            do! DB.addMultiple(
                    [| { LocationId = "X1"; Name = "Bell tower"; Address = "Market place" } 
                       { LocationId = "X2"; Name = "Graveyard"; Address = "Right next to church" } |])
    } 
    |> Async.StartImmediate
    
let saveLocationCheckResult data =
    async {
        try
            let! newID = DB.add<Model.LocationCheckResult> data
            ()
        with
        | error -> () // Error saving data
    } |> Async.StartImmediate