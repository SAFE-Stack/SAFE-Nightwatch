module Database

open Fable.Core
open Fable.ReactNative.SimpleStore.DB
open Model

[<Emit("require($0)")>]
let private localResource (path: string) : 'T = jsNative

let createDemoData (data: Shared.LocationCheckRequest []) =
    promise {
        try
            do! clear<LocationCheckRequest> ()
            // Fetch demo data
            do! addMultiple data
            return data.Length
        with _ -> return 0
    }

let getIndexedCheckRequests () =
    getAll<Model.LocationCheckRequest> ()
    |> Promise.map (Array.mapi (fun i r -> i, r))
