module CheckLocation

open System
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore
open Fable.Helpers.ReactNativeImagePicker
open Fable.Helpers.ReactNativeImagePicker.Props
open Elmish

// Model
type Status =
| Unchanged
| Changed
| Error of string

type Msg =
| PictureSelected of string option
| LocationStatusUpdated of Model.LocationStatus
| SelectPicture
| SaveAndGoBack
| GoBack
| Error of exn

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
        [ Title "Take picture"
          AllowsEditing true]

// Update
let update msg model : Model*Cmd<Msg> =
    match msg with
    | SaveAndGoBack ->
        match model.Status with
        | Unchanged -> model, Cmd.ofMsg GoBack
        | _ ->
            model, Cmd.ofPromise save (model.Position,model.LocationCheckRequest) (fun _ -> GoBack) Error

    | GoBack ->
        model, Cmd.none // Handled one level above

    | PictureSelected selectedPicture ->
        { model with
            PictureUri = selectedPicture
            Status = Changed }, Cmd.none

    | SelectPicture ->
        model,Cmd.ofPromise selectImage () PictureSelected Error

    | LocationStatusUpdated newStatus ->
        { model with
            LocationCheckRequest =
                { model.LocationCheckRequest with
                      Status = Some newStatus
                      Date = Some DateTime.Now }
            Status = Changed }, Cmd.none

    | Error e ->
        Toast.showShort e.Message
        { model with
            Status = Status.Error e.Message }, Cmd.none

// View
let view (model:Model) (dispatch: Msg -> unit) =
    let selectImageButton =
        Styles.button "Take picture"
            (fun () -> dispatch SelectPicture)

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
                [ Source (localImage "${entryDir}/../images/snow.jpg")
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
            TextInput.TextInputProperties.OnChangeText (Model.LocationStatus.Alarm >> LocationStatusUpdated >> dispatch)
          ] (match model.LocationCheckRequest.Status with
             | Some (Model.LocationStatus.Alarm s) -> s
             | _ -> "")

          image
          selectImageButton
          view
            [ ViewProperties.Style [
                FlexStyle.JustifyContent JustifyContent.Center
                FlexStyle.AlignItems ItemAlignment.Center
                FlexStyle.Flex 1.
                FlexStyle.FlexDirection FlexDirection.Row ]]
              [ Styles.button "Cancel" (fun () -> dispatch GoBack)
                Styles.button "OK" (fun () -> dispatch SaveAndGoBack) ]
        ]
