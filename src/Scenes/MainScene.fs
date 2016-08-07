module internal MainScene

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

type MainSceneProperties = {
    Navigator:Navigator
}

type MainScene (props) =
    inherit React.Component<MainSceneProperties,obj>(props)

    member x.render () =
        let logo =
            image 
                [ Source (localImage "../../images/raven.jpg")
                  ImageProperties.Style [
                    ImageStyle.AlignSelf Alignment.Center
                    ImageStyle.FlexDirection FlexDirection.Column
                  ]
                ]

        view [ Styles.sceneBackground ] 
          [ text [ Styles.titleText ] "Nightwatch"
            logo
            Styles.button "Begin watch" (fun () -> x.props.Navigator.push (createRoute("LocationListScene",1))) ]