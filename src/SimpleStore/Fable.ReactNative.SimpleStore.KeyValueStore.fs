module Fable.ReactNativeSimpleStore.KeyValueStore
open Fable.Core.JsInterop

[<AutoOpen>]
module Helpers =
    let private asyncStorage : obj = importDefault "@react-native-community/async-storage"

    /// Retrieves all keys from the AsyncStorage.
    let getAllKeys() : Fable.Core.JS.Promise<string []> =
        Promise.create(fun success fail ->
            asyncStorage?getAllKeys
                (fun err keys ->
                    if not (isNull err) && not (isNull (err?message)) then
                        fail (unbox err)
                    else
                        success (unbox keys)) |> ignore)