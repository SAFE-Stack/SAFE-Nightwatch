module Home

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Elmish
open Messages

// Model
type Model = { StatusText : string  }

// Update
let update (msg:HomeSceneMsg) model : Model*Cmd<AppMsg> = 
    match msg with
    | HomeSceneMsg.GetDemoData ->
        { model with StatusText = "Syncing..." }, Cmd.ofAsync Database.createDemoData () HomeSceneMsg.NewDemoData HomeSceneMsg.Error |> Cmd.map HomeSceneMsg 
    | HomeSceneMsg.NewDemoData count ->
        { model with StatusText = sprintf "Locations: %d" count }, []
    | HomeSceneMsg.BeginWatch ->
        { model with StatusText = "" }, Cmd.ofMsg (AppMsg.NavigateTo Page.LocationList)
    | HomeSceneMsg.Error e ->
        { model with StatusText = string e.Message }, []


let init () = { StatusText = "" }, Cmd.ofMsg HomeSceneMsg.GetDemoData |> Cmd.map HomeSceneMsg

// View 
let view (model:Model) (dispatch: AppMsg -> unit) =
      let logo =
          image 
              [ Source (localImage "../../images/raven.jpg")
                ImageProperties.Style [
                  FlexStyle.AlignSelf Alignment.Center
                  FlexStyle.FlexDirection FlexDirection.Column
                ]
              ]

      view [ Styles.sceneBackground ] 
        [ text [ Styles.titleText ] "Nightwatch"
          logo
          Styles.button "Begin watch" (fun () -> dispatch (HomeSceneMsg (HomeSceneMsg.BeginWatch)))
          Styles.whitespace
          Styles.whitespace
          text [ Styles.smallText ] model.StatusText  ]
