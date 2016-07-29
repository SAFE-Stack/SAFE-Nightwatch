module ImagePickerApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactNativeImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeImagePicker
open Fable.Helpers.ReactNativeImagePicker.Props

type RN = ReactNative.Globals
type IP = ReactNativeImagePicker.Globals


type ImagePickerAppState = {
    uri: string
}


let baseUrl = "http://facebook.github.io/react/img/logo_og.png"

type ImagePickerApp (props) as this =
    inherit React.Component<obj,ImagePickerAppState>(props)
    do this.state <- { uri = baseUrl }

    member x.render () =

        let button =
            text [] "click me to select image!"
            |> touchableHighlight [
                TouchableHighlightProperties.Style [
                    ViewStyle.BackgroundColor "#AA00AA"
                    ViewStyle.Flex 1
                 ]
                OnPress 
                    (fun () ->
                        (showImagePicker
                            [Title "Image picker"; AllowsEditing true] 
                            (fun result -> 
                                x.setState { 
                                    uri = 
                                        if not result.didCancel then
                                            if String.IsNullOrEmpty result.error then
                                                result.uri
                                            else
                                                result.error
                                        else
                                            baseUrl })))]


        let image =
            image 
                [ Source [ Uri x.state.uri; IsStatic true]
                  Style [
                    ImageStyle.BorderColor "#000000"
                    ImageStyle.Flex 7
                  ]
                ]
                [ ]


        view [ ViewProperties.Style [ViewStyle.Flex 1]] 
         [ image
           button ]
