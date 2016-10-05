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

let createDemoData() =
    async { 
        try
            do! DB.clear<LocationCheckRequest>()
            // Fetch demo data 
            let! requests =
                fetchAs<LocationCheckRequest[]>
                    ("https://raw.githubusercontent.com/fsprojects/fable-react_native-demo/master/demodata/LocationCheckRequests.json",
                    [])
            do! DB.addMultiple requests
            return requests.Length
        with
        | error -> return 0
    } 


let getIndexedCheckRequests () =
    async {
        let! requests = DB.getAll<Model.LocationCheckRequest>()
        return 
            requests
            |> Array.mapi (fun i r -> i,r)
    }