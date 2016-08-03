module ImagePickerScene

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactNativeImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeImagePicker
open Fable.Helpers.ReactNativeImagePicker.Props

type ImagePickerState = {
    uri: string
}

type ImagePickerSceneProperties = {
    initState : ImagePickerState
    onDone: ImagePickerState -> unit
}

type ImagePickerScene (props) as this =
    inherit React.Component<ImagePickerSceneProperties,ImagePickerState>(props)

    do this.state <- this.props.initState

    member x.render () =
        let selectImageButton =
            Styles.button "Click me to select image!"
                (fun () ->
                    (showImagePicker
                        [Title "Image picker"; AllowsEditing true] 
                        (fun result ->    
                            if not result.didCancel then
                                if String.IsNullOrEmpty result.error then
                                        x.setState { uri = result.uri } )))
        let image =
            image 
                [ Source [ Uri x.state.uri; IsStatic true]
                  ImageProperties.Style [
                    ImageStyle.BorderColor "#000000"
                    ImageStyle.Flex 3
                  ]
                ]

        view [ Styles.sceneBackground ]
            [ image
              selectImageButton
              Styles.button "Tap me to go back" (fun () -> x.props.onDone x.state) ]
