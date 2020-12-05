module Fable.ReactNative.SimpleStore.DB
open Fable.Core.JsInterop
open Thoth.Json
    
[<AutoOpen>]  
module Helpers =
    
    let inline toJsonWithTypeInfo value = Encode.Auto.toString(0, value)
    let AsyncStorage: obj = importDefault "@react-native-community/async-storage"
     
    [<Literal>]
    let ModelsKey = "models/"
    type Table<'a> = 'a[]

    let inline private setItem(key, s): Fable.Core.JS.Promise<unit> =
        unbox(AsyncStorage?setItem(key,s))

    let inline private removeItem(key): Fable.Core.JS.Promise<unit> =
        unbox(AsyncStorage?removeItem key)

    /// Removes all rows from the model.
    let inline clear<'a>() =
        let key = ModelsKey + typeof<'a>.FullName
        let s:string = [||] |> toJsonWithTypeInfo
        setItem(key,s)

    /// Gets or creates a new model.
    let inline private getModel<'a> (key) : Fable.Core.JS.Promise<Table<'a>> =
        AsyncStorage?getItem (key)
        |> Promise.map (function
            | null -> [||]
            | v -> Decode.Auto.unsafeFromString v)

    /// Adds a row to a model
    let inline add<'a> (data:'a) =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key
        |> Promise.bind (fun model ->
            let newModel : string = Array.append [|unbox data|] model |> toJsonWithTypeInfo
            setItem(key,newModel))

    /// Updates a row in a model
    let inline update<'a>(index, data:'a) =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key
        |> Promise.bind (fun model ->
            model.[index] <- unbox data
            let newModel : string =  toJsonWithTypeInfo model
            setItem(key,newModel))

    /// Deletes a row from a model
    let inline delete<'a>(index) =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key
        |> Promise.bind (fun model ->
            let model : 'a[] =
                model
                |> Array.mapi (fun i x -> i,x)
                |> Array.filter (fun (i,_) -> i <> index)
                |> Array.map snd
            let newModel : string =  toJsonWithTypeInfo model
            setItem(key,newModel))

    /// Updates multiple rows in a model
    let inline updateMultiple<'a>(values) =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key
        |> Promise.bind (fun model ->
            for index, data:'a in values do
                model.[index] <- unbox data
            let newModel : string =  toJsonWithTypeInfo model
            setItem(key,newModel))

    ///  Update data by an update function.
    let inline updateWithFunction<'a>(updateF: 'a[] -> 'a[]) =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key
        |> Promise.bind (fun model ->
            let updated = updateF model
            let newModel : string = toJsonWithTypeInfo updated
            setItem(key,newModel))

    ///  Update data by an update function.
    let inline updateWithFunctionAndKey<'a>(updateF: 'a[] -> 'a[],key) =
        let key = ModelsKey + typeof<'a>.FullName + "/" + key
        getModel<'a> key
        |> Promise.bind (fun model ->
            let updated = updateF model
            let newModel : string = toJsonWithTypeInfo updated
            setItem(key,newModel))

    /// Adds multiple rows to a model
    let inline addMultiple<'a>(data:'a []) =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key
        |> Promise.bind (fun model ->
            let newModel : string = Array.append data model |> toJsonWithTypeInfo
            setItem(key,newModel))

    /// Replaces all rows of a model
    let inline replaceWithKey<'a>(key,data:'a []) =
        let modelKey = ModelsKey + typeof<'a>.FullName + "/" + key
        let newModel : string = data |> toJsonWithTypeInfo
        setItem(modelKey,newModel)

    /// Deletes all rows of a model
    let inline deleteWithKey<'a>(key) =
        let modelKey = ModelsKey + typeof<'a>.FullName + "/" + key
        removeItem(modelKey)

    /// Replaces all rows of a model
    let inline replace<'a>(data:'a []) =
        let modelKey = ModelsKey + typeof<'a>.FullName
        let newModel : string = data |> toJsonWithTypeInfo
        setItem(modelKey,newModel)

    /// Gets a row from the model
    let inline get<'a>(index:int) =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key
        |> Promise.map (fun model -> model.[index])

    /// Gets all rows from the model
    let inline getAll<'a>() =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key

    /// Gets all rows from the model
    let inline getAllWithKey<'a>(key) =
        let key = ModelsKey + typeof<'a>.FullName + "/" + key
        getModel<'a> key

    /// Gets the row count from the model
    let inline count<'a>() =
        let key = ModelsKey + typeof<'a>.FullName
        getModel<'a> key
        |> Promise.map (fun model -> model.Length)