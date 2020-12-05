module Database

open Fable.Core
open Fable.ReactNative.SimpleStore.DB
open Model
open Thoth.Json
[<Emit("require($0)")>]
let private localResource(path:string): 'T = jsNative

let createDemoData() =
    promise {
        try
            do! clear<LocationCheckRequest>()
            // Fetch demo data
            let requests: LocationCheckRequest[] =
                localResource "${entryDir}/../demodata/LocationCheckRequests.json"
            do! addMultiple requests  
            return requests.Length
        with
        | _ -> return 0
    }

let getIndexedCheckRequests () = 
    getAll<Model.LocationCheckRequest>()
    |> Promise.map (Array.mapi (fun i r -> i,r))
