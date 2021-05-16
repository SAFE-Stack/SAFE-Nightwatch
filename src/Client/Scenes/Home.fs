module Home

open Fable.ReactNative
open Fable.ReactNative.Props
open Elmish
open Shared
open SimpleToast
// Model

type Model = { 
    StatusText : string
    Data : LocationCheckRequest []  }

type Msg =
| GetDemoData
| NewDemoData of LocationCheckRequest []
| AddedDemoDataToLocalStore of int
| BeginWatch
| Error of exn

// Update
let update (msg:Msg) model : Model*Cmd<Msg> =
    match msg with
    | GetDemoData ->
        printfn "Getting data"
        { model with StatusText = "Syncing..." },
        Cmd.OfAsync.either FetchData.locationApi.getLocation () NewDemoData Error

    | NewDemoData locations ->
        printfn "Got Locations from Server"
        { model with 
            StatusText = "Got Data"
            Data = locations }, 
        Cmd.OfPromise.either Database.createDemoData locations AddedDemoDataToLocalStore Error

    | AddedDemoDataToLocalStore i ->
        { model with 
            StatusText = sprintf "Locations: %d" i},  Cmd.none 

    | BeginWatch ->
        model, Cmd.none // Handled one level above

    | Error e ->
        // Toaster.showShort e.Message
        { model with StatusText = string e.Message }, Cmd.none


let init () = { StatusText = ""; Data =[||] }, Cmd.ofMsg GetDemoData

// View

let view (model:Model) (dispatch: Msg -> unit) =
      printfn "%A" (localImage "../../images/raven.jpg")
      let logo =
          image
              [ Source (localImage "../../images/raven.jpg")
                ImageProperties.Style [
                  FlexStyle.AlignSelf Alignment.Center
                  FlexStyle.FlexDirection FlexDirection.Column
                ]
              ]
 
      view [ Styles.sceneBackground ]
        [ text [ Styles.titleText ] "Client"
          logo
          Styles.button "Begin watch" (fun () -> dispatch BeginWatch)
          Styles.whitespace
          Styles.whitespace
          text [ Styles.smallText ] model.StatusText  ]
