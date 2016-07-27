module HelloWorld

open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactImageCropPicker

type RN = ReactNative.Globals
type IP = ReactImageCropPicker.Globals

type HelloWorldApp (props) =
    inherit React.Component<obj,obj>(props)

    member x.render () =
        let buttonProps =
            let t = createEmpty<TextProperties>
            let p = createEmpty<ImagePickerOptions>
            t.onPress <- unbox (fun () -> IP.ImagePicker.openPicker(p) )
            t

        React.createElement(RN.View, unbox null,
            [|
                React.createElement(RN.Text, buttonProps, unbox "Hello World") |> unbox
                //React.createElement(RN.TouchableHighlight, buttonProps, unbox "Click me" ) |> unbox
            |]
        )