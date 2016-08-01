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
        let nextButton =
            text [] "Tap me to go to image picker scene"
            |> touchableHighlight [OnPress x.props.onGoToImagePicker]
        view [  ]
          [ text [] "Image picker app"
            nextButton ]