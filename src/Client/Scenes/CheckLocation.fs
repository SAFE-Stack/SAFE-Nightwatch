module CheckLocation

open System
open Fable.ReactNative
open Fable.ReactNative.Props
open Fable.ReactNativeImagePicker
open Fable.ReactNativeImagePicker.Props
open Elmish
open Fable.ReactNative.Props.TextInput
open Fable.ReactNative.SimpleStore.DB
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
      

let save (pos,request : Model.LocationCheckRequest) = update(pos,request)

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
            model, Cmd.OfPromise.either save (model.Position,model.LocationCheckRequest) (fun _ -> GoBack) Error

    | GoBack ->
        model, Cmd.none // Handled one level above

    | PictureSelected selectedPicture ->
        { model with
            PictureUri = selectedPicture
            Status = Changed }, Cmd.none

    | SelectPicture ->
        model, Cmd.OfPromise.either selectImage () PictureSelected Error

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
                [ Source (remoteImage [Uri uri])
                  ImageProperties.Style [
                    BorderColor "#000000"
                    Flex 3.
                  ]
                ]
        | None ->
            image
                [ Source (localImage "../../images/snow.jpg")
                  ImageProperties.Style [
                    BorderColor "#000000"
                    Flex 3.
                    AlignSelf Alignment.Center
                  ]
            ]

    view [ Styles.sceneBackground ]
        [ text [ Styles.defaultText ] model.LocationCheckRequest.Name
          textInput [
            AutoCorrect false
            TextInputProperties.Style [
                MarginTop (unbox 2.)
                MarginBottom (unbox 2.)
                TextStyle.Color Styles.textColor
                ViewStyle.BackgroundColor Styles.inputBackgroundColor
              ]
            OnChangeText (Model.LocationStatus.Alarm >> LocationStatusUpdated >> dispatch)
            Value
              (match model.LocationCheckRequest.Status with
               | Some (Model.LocationStatus.Alarm s) -> s
               | _ -> "")
          ] 

          image
          selectImageButton
          view
            [ ViewProperties.Style [
                JustifyContent JustifyContent.Center
                AlignItems ItemAlignment.Center
                Flex 1.
                FlexDirection FlexDirection.Row ]]
              [ Styles.button "Cancel" (fun () -> dispatch GoBack)
                Styles.button "OK" (fun () -> dispatch SaveAndGoBack) ]
        ]
