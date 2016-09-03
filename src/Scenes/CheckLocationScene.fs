module internal CheckLocationScene

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
open Fable.Core.JsInterop

type CheckLocationSceneProperties = {
    Navigator: Navigator
    Route: RouteResult
    ReadingRequest: Model.LocationCheckRequest
}

type CheckLocationScene (props) as this =
    inherit React.Component<CheckLocationSceneProperties,Model.LocationCheckResult>(props)

    do this.state <- Model.LocationCheckResult.FromRequest this.props.ReadingRequest

    member x.render () =
        let selectImageButton =
            Styles.button "Take picture"
                (fun () ->
                    (showImagePicker
                        [AllowsEditing true] 
                        (fun result ->
                            if not result.didCancel && 
                               String.IsNullOrEmpty result.error 
                            then
                               x.setState { x.state with PictureUri = Some result.uri } )))

        let image =
            match x.state.PictureUri with
            | Some uri -> 
                image
                    [ Source [ Uri uri; IsStatic true]
                      ImageProperties.Style [
                        ImageStyle.BorderColor "#000000"
                        ImageStyle.Flex 3
                      ]
                    ]                
            | None -> 
                image
                    [ Source (localImage "../../images/snow.jpg")
                      ImageProperties.Style [
                        ImageStyle.BorderColor "#000000"
                        ImageStyle.Flex 3
                        ImageStyle.AlignSelf Alignment.Center
                      ]
                ]

        view [ Styles.sceneBackground ] 
            [ text [ Styles.defaultText ] x.props.ReadingRequest.Name
              textInput [
                TextInputProperties.AutoCorrect false
                TextInputProperties.Style [
                    TextStyle.MarginTop 2.
                    TextStyle.MarginBottom 2.
                    TextStyle.Color Styles.textColor
                    TextStyle.BackgroundColor Styles.inputBackgroundColor
                  ]
                TextInputProperties.OnChangeText 
                    (fun text ->
                        let status =
                            if String.IsNullOrWhiteSpace text then
                                Model.LocationStatus.Ok
                            else
                                Model.LocationStatus.Alarm text
                        x.setState { x.state with Status = status })
              ] ""

              image
              selectImageButton
              view 
                [ ViewProperties.Style [
                    ViewStyle.JustifyContent JustifyContent.Center
                    ViewStyle.AlignItems ItemAlignment.Center
                    ViewStyle.Flex 1
                    ViewStyle.FlexDirection FlexDirection.Row ]]
                  [ Styles.verticalButton "Cancel" 
                        (fun () -> 
                             x.props.Route.onCancelButton()
                             x.props.Navigator.pop())
                    Styles.verticalButton "OK" 
                        (fun () ->
                            Database.saveLocationCheckResult x.state
                            
                            x.props.Route.onOkButton()
                            x.props.Navigator.pop())]
            ]
