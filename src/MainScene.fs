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

        view [ Styles.getSceneBackground() ] 
          [ text [ Styles.getTitleText() ] "Image picker"
            logo
            Styles.button "Tap me to go to image picker scene" x.props.onGoToImagePicker ]