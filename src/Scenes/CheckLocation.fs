module CheckLocation

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactNativeImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore
open Fable.Helpers.ReactNativeImagePicker
open Fable.Helpers.ReactNativeImagePicker.Props
open Fable.Import.JS
open Elmish
open Messages

// Model
type Status =
| Unchanged
| Changed
| Error of string

type Model =
  { Position : int
    LocationCheckRequest : Model.LocationCheckRequest
    PictureUri : string option
    Status : Status }

let init (pos,request) =
    { Status = Unchanged
      Position = pos
      PictureUri = None
      LocationCheckRequest = request }, Cmd.none

// Helpers update
let save (pos,request : Model.LocationCheckRequest) = DB.update(pos,request)

let selectImage () =
    showImagePickerAsync
        [Title "Take picture"
         AllowsEditing true]

// Update
let update msg model : Model*Cmd<AppMsg> =
    match msg with
    | LocationCheckMsg.SaveAndGoBack ->
        match model.Status with
        | Unchanged -> model,Cmd.ofMsg LocationCheckMsg.GoBack |> Cmd.map LocationCheckMsg
        | _ ->
            let saveAndGoBack = Cmd.ofPromise save (model.Position,model.LocationCheckRequest) (fun _ -> LocationCheckMsg.GoBack) LocationCheckMsg.Error |> Cmd.map LocationCheckMsg
            let sendAlarm =
                match model.LocationCheckRequest.Status with
                | Some(Model.LocationStatus.Alarm text) -> Cmd.ofMsg (AppMsg.SendAlarm (sprintf "Alarm at %s. Message: %s" model.LocationCheckRequest.Name text))
                | _ -> Cmd.none

            model,Cmd.batch [saveAndGoBack; sendAlarm]
    | LocationCheckMsg.GoBack ->
        model, Cmd.ofMsg AppMsg.NavigateBack
    | LocationCheckMsg.PictureSelected selectedPicture ->
        { model with
            PictureUri = selectedPicture
            Status = Changed }, []
    | LocationCheckMsg.SelectPicture ->
        model,Cmd.ofPromise selectImage () LocationCheckMsg.PictureSelected LocationCheckMsg.Error |> Cmd.map LocationCheckMsg
    | LocationCheckMsg.LocationStatusUpdated newStatus ->
        { model with
            LocationCheckRequest =
                { model.LocationCheckRequest with
                      Status = Some newStatus
                      Date = Some DateTime.Now }
            Status = Changed }, []
    | LocationCheckMsg.Error e ->
        { model with
            Status = Error (string e.Message) }, []

// View
let view (model:Model) (dispatch: AppMsg -> unit) =
    let selectImageButton =
        Styles.button "Take picture"
            (fun () -> dispatch (LocationCheckMsg LocationCheckMsg.SelectPicture))

    let image =
        match model.PictureUri with
        | Some uri ->
            image
                [ Source [ Uri uri; IsStatic true]
                  ImageProperties.Style [
                    ImageStyle.BorderColor "#000000"
                    FlexStyle.Flex 3.
                  ]
                ]
        | None ->
            image
                [ Source (localImage "../images/snow.jpg")
                  ImageProperties.Style [
                    ImageStyle.BorderColor "#000000"
                    FlexStyle.Flex 3.
                    FlexStyle.AlignSelf Alignment.Center
                  ]
            ]

    view [ Styles.sceneBackground ]
        [ text [ Styles.defaultText ] model.LocationCheckRequest.Name
          textInput [
            TextInput.TextInputProperties.AutoCorrect false
            TextInput.TextInputProperties.Style [
                FlexStyle.MarginTop 2.
                FlexStyle.MarginBottom 2.
                TextStyle.Color Styles.textColor
                ViewStyle.BackgroundColor Styles.inputBackgroundColor
              ]
            TextInput.TextInputProperties.OnChangeText (Model.LocationStatus.Alarm >> LocationCheckMsg.LocationStatusUpdated >> LocationCheckMsg >> dispatch)
          ] ""

          image
          selectImageButton
          view
            [ ViewProperties.Style [
                FlexStyle.JustifyContent JustifyContent.Center
                FlexStyle.AlignItems ItemAlignment.Center
                FlexStyle.Flex 1.
                FlexStyle.FlexDirection FlexDirection.Row ]]
              [ Styles.verticalButton "Cancel" (fun () -> dispatch NavigateBack)
                Styles.verticalButton "OK" (fun () -> dispatch (LocationCheckMsg LocationCheckMsg.SaveAndGoBack)) ]
        ]
