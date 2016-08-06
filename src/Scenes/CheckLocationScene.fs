module internal CheckLocationScene

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactNativeImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeImagePicker
open Fable.Helpers.ReactNativeImagePicker.Props

type CheckLocationSceneProperties = {
    Navigator: Navigator
    Route: RouteResult
    ReadingRequest: Model.LocationCheckRequest
}

type CheckLocationScene (props) as this =
    inherit React.Component<CheckLocationSceneProperties,Model.LocationCheckResult>(props)

    do this.state <- Model.LocationCheckResult.FromRequest this.props.ReadingRequest

    member x.render () =

        let image =
            let image =
                match x.state.PictureUri with
                | Some uri -> 
                    image
                        [ Source [ Uri uri; IsStatic true]
                          ImageProperties.Style [
                            ImageStyle.Flex 3
                            ImageStyle.AlignSelf Alignment.Center
                          ]
                    ]
                | None ->
                    imageWithChild
                        [ Source (localImage "../../images/snow.jpg")
                          ImageProperties.Style [
                            ImageStyle.Flex 3
                            ImageStyle.AlignSelf Alignment.Center
                          ]
                        ] 
                        (text [ Styles.defaultText ] "Tap to select picture")

            image
            |> touchableHighlight [
                TouchableHighlightProperties.UnderlayColor Styles.touched
                OnPress 
                  (fun () ->
                    (showImagePicker
                        [AllowsEditing true] 
                        (fun result ->    
                            if not result.didCancel then
                                if String.IsNullOrEmpty result.error then
                                        x.setState { x.state with PictureUri = Some result.uri } )))]

        view [ Styles.sceneBackground ] 
            [ text [ Styles.defaultText ] ("Location: " + x.props.ReadingRequest.Name)
              text [ Styles.defaultText ] x.props.ReadingRequest.Address
              textInput [
                TextInputProperties.KeyboardType KeyboardType.Numeric
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
              view [ 
                  ViewProperties.Style[
                    ViewStyle.JustifyContent Alignment.Center
                    ViewStyle.AlignItems ItemAlignment.Center
                    ViewStyle.Flex 1
                    ViewStyle.FlexDirection FlexDirection.Row ]]
                  [ Styles.verticalButton "Cancel" (fun () -> x.props.Route.onCancelButton(); x.props.Navigator.pop())
                    Styles.verticalButton "OK" 
                        (fun () ->
                            Database.saveLocationCheckResult x.state
                            
                            x.props.Route.onOkButton()
                            x.props.Navigator.pop()) ]
              ]
