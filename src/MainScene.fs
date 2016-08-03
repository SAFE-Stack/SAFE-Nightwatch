module MainScene

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

type MainSceneProperties = {
    onGoToImagePicker: unit -> unit
}

type MainScene (props) =
    inherit React.Component<MainSceneProperties,obj>(props)

    member x.render () =
        let logo =
            image 
                [ Source (localImage "../images/logo.png")
                  ImageProperties.Style [
                    ImageStyle.AlignSelf Alignment.Center
                    ImageStyle.MarginBottom 15.
                  ]
                ]

        let nextButton =
            text [ Styles.defaultText ] "Tap me to go to image picker scene"
            |> touchableHighlight [
                Styles.buttonStyle
                TouchableHighlightProperties.UnderlayColor Styles.touched
                OnPress x.props.onGoToImagePicker]

        view [ Styles.sceneBackground ] 
          [ text [ Styles.titleText ] "Image picker app"
            logo
            nextButton ]