module MainScene

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

type MainSceneProperties = {
    Navigator : Navigator
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

        view [ Styles.sceneBackground ] 
          [ text [ Styles.titleText ] "Image picker"
            logo
            Styles.button "Tap me to go to image picker scene" (fun () -> x.props.Navigator.push (createRoute("ImagePicker",1))) ]