module ImagePickerApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactImagePicker

type RN = ReactNative.Globals
type IP = ReactImagePicker.Globals

type ImagePickerAppState = {
    uri: string
}

type ImageSource =
    abstract uri: string option with get, set
    abstract isStatic: bool option with get, set        

type ImagePickerApp (props) as this =
    inherit React.Component<obj,ImagePickerAppState>(props)
    do this.state <- { uri = "" }

    member x.render () =

        let buttonProps =
             let t = createEmpty<TouchableHighlightProperties>
             t.onPress 
                <- (fun () -> 
                        let p = createEmpty<ImagePickerOptions>
                        p.title <- unbox "Image picker"
                        p.allowsEditing <- unbox true

                        IP.ImagePicker.showImagePicker(p, fun result -> 
                            x.setState { 
                                uri = 
                                    if not result.didCancel then
                                        if String.IsNullOrEmpty result.error then
                                            result.uri
                                        else
                                            result.error
                                    else
                                        "dialog canceled" }))
                    |> unbox
             t


        let highlight = React.createElement(RN.TouchableHighlight, buttonProps, [| React.createElement(RN.Text, unbox null, unbox "click me") |> unbox |])



        let imageProps = 
            let p = createEmpty<ImageProperties>
            let source = createEmpty<ImageSource>
            source.uri <- Some x.state.uri
            source.isStatic <- Some true

            p.source <- unbox source
            p

        React.createElement(RN.View, unbox null,
            [|
                highlight |> unbox
                React.createElement(RN.Image, unbox imageProps, [| React.createElement(RN.Text, unbox null, unbox "") |> unbox |])  |> unbox
            |]
        )