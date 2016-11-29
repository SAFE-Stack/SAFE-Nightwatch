module Database

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore
open Fable.PowerPack
open Model

let createDemoData() =
    promise {
        try
            do! DB.clear<LocationCheckRequest>()
            // Fetch demo data
            let! requests =
                Fetch.fetchAs<LocationCheckRequest[]>
                    "https://raw.githubusercontent.com/fsprojects/fable-react_native-demo/master/demodata/LocationCheckRequests.json" []
            do! DB.addMultiple requests
            return requests.Length
        with
        | error -> return 0
    }

let getIndexedCheckRequests () =
    DB.getAll<Model.LocationCheckRequest>()
    |> Promise.map (Array.mapi (fun i r -> i,r))
