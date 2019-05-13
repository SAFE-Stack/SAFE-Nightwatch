module Database

open Fable.Core
open Fable.ReactNativeSimpleStore.DB
open Model

[<Emit("require($0)")>]
let private localResource(path:string): 'T = jsNative

let createDemoData() =
    promise {
        try
            do! clear<LocationCheckRequest>()
            // Fetch demo data
            let requests: LocationCheckRequest[] =
                localResource "${entryDir}/../demodata/LocationCheckRequests.json"
            // let! requests =
            //     Fetch.fetchAs<LocationCheckRequest[]>
            //         "https://raw.githubusercontent.com/fsprojects/fable-react_native-demo/master/demodata/LocationCheckRequests.json" []
            do! addMultiple requests
            return requests.Length
        with
        | _ -> return 0
    }

let getIndexedCheckRequests () =
    getAll<Model.LocationCheckRequest>()
    |> Promise.map (Array.mapi (fun i r -> i,r))
