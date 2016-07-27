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
             let t = createEmpty<TextProperties>
             let p = createEmpty<ImagePickerOptions>             
             p.title <- unbox "Meter device"
             p.allowsEditing <- unbox true

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
                                        "dialog canceld" }))
                    |> unbox
             t

        let textBox = React.createElement(RN.Text, buttonProps, unbox x.state.uri) 

        React.createElement(RN.View, unbox null,
            [|
                textBox |> unbox
            |]
        )