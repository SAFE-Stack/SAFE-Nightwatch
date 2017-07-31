module Home

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Elmish

// Model
type Msg =
| GetDemoData
| NewDemoData of int
| BeginWatch
| Error of exn

type Model = { StatusText : string  }

// Update
let update (msg:Msg) model : Model*Cmd<Msg> =
    match msg with
    | GetDemoData ->
        { model with StatusText = "Syncing..." },
        Cmd.ofPromise Database.createDemoData () NewDemoData Error

    | NewDemoData count ->
        { model with StatusText = sprintf "Locations: %d" count }, Cmd.none

    | BeginWatch ->
        model, Cmd.none // Handled one level above

    | Error e ->
        { model with StatusText = string e.Message }, Cmd.none


let init () = { StatusText = "" }, Cmd.ofMsg GetDemoData

// View
let view (model:Model) (dispatch: Msg -> unit) =
      let logo =
          image
              [ Source (localImage "${entryDir}/../images/raven.jpg")
                ImageProperties.Style [
                  FlexStyle.AlignSelf Alignment.Center
                  FlexStyle.FlexDirection FlexDirection.Column
                ]
              ]

      view [ Styles.sceneBackground ]
        [ text [ Styles.titleText ] "Nightwatch"
          logo
          Styles.button "Begin watch" (fun () -> dispatch BeginWatch)
          Styles.whitespace
          Styles.whitespace
          text [ Styles.smallText ] model.StatusText  ]
