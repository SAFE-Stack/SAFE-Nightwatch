module HelloWorld

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactImagePicker

type RN = ReactNative.Globals
type IP = ReactImagePicker.Globals

type HelloWorldState = {
    uri: string
}

type HelloWorldApp (props) as this =
    inherit React.Component<obj,HelloWorldState>(props)
    do this.state <- { uri = "Click me" }

    member x.render () =

        let buttonProps =
             let t = createEmpty<TouchableHighlightProperties>
             t.onPress 
                <- (fun () -> 
                        let p = createEmpty<ImagePickerOptions>
                        p.title <- unbox "Meter device"
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


        let highlight = React.createElement(RN.TouchableHighlight, buttonProps, [| React.createElement(RN.Text, unbox null, unbox x.state.uri) |> unbox |])      

        React.createElement(RN.View, unbox null,
            [|
                highlight |> unbox
            |]
        )