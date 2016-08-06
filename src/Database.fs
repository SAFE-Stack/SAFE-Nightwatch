module Database

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore
open AsyncPromiseExtensions
open Fable.Import.Fetch

let getRequestsAndMatchingResults() =
    async { 
        let! requests = DB.getAll<Model.LocationCheckRequest>()
        let! results = DB.getAll<Model.LocationCheckResult>()

        return
            requests
            |> Array.map (fun request -> request,results |> Array.tryFind (fun result -> result.LocationId = request.LocationId))
    }

let createDemoData() =
    async { 
        try
            let! count = DB.count<Model.LocationCheckRequest>()
            if count = 0 then
                // Fetch demo data 
                let! fetched = GlobalFetch.fetch (Url "https://raw.githubusercontent.com/fsprojects/fable-react_native-demo/master/demodata/LcoationCheckRequests.json")
                let! json = fetched.json()
                let requests : Model.LocationCheckRequest[] = unbox json
                do! DB.addMultiple<Model.LocationCheckRequest>(requests)
        with
        | error -> 
            do! DB.addMultiple<Model.LocationCheckRequest>(
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