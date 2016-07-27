module HelloWorld

open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactImagePicker

type RN = ReactNative.Globals
type IP = ReactImagePicker.Globals

type HelloWorldApp (props) =
    inherit React.Component<obj,obj>(props)

    member x.render () =

        let buttonProps =
            let t = createEmpty<TextProperties>
            let p = createEmpty<ImagePickerOptions>
            p.title <- unbox "Select meter picture"
            t.onPress <- unbox (fun () -> IP.ImagePicker.showImagePicker(p, fun result -> () ))
            t

        let textBox = React.createElement(RN.Text, buttonProps, unbox "Hello World") 

        React.createElement(RN.View, unbox null,
            [|
                textBox |> unbox
            |]
        )