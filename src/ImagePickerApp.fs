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

type ReactElement = obj

let createText properties text : ReactElement = React.createElement(RN.Text, properties, unbox text) |> unbox

let createTouchableHighlight properties (child:ReactElement) : ReactElement  = React.createElement(RN.TouchableHighlight, properties, [| unbox child |]) |> unbox

let createView properties (childs:ReactElement []) : ReactElement = React.createElement(RN.View, properties, childs |> unbox) |> unbox

let createImage properties (childs:ReactElement []) : ReactElement = React.createElement(RN.Image, properties, childs |> unbox) |> unbox 

type ImageSource =
    abstract uri: string option with get, set
    abstract isStatic: bool option with get, set        

let baseUrl = "http://facebook.github.io/react/img/logo_og.png"

type ImagePickerApp (props) as this =
    inherit React.Component<obj,ImagePickerAppState>(props)
    do this.state <- { uri = baseUrl }

    member x.render () =

        let buttonProps =
             let t = createEmpty<TouchableHighlightProperties>
             t.onPress 
                <- (fun () -> 
                        let p = createEmpty<ImagePickerOptions>
                        p.title <- Some "Image picker"
                        p.allowsEditing <- Some true

                        IP.ImagePicker.showImagePicker(p, fun result -> 
                            x.setState { 
                                uri = 
                                    if not result.didCancel then
                                        if String.IsNullOrEmpty result.error then
                                            result.uri
                                        else
                                            result.error
                                    else
                                        baseUrl }))
                    |> unbox
             t


        let button =
            createText (unbox null) "click me to select image!"
            |> createTouchableHighlight buttonProps

        let imageProps = 
            let p = createEmpty<ImageProperties>
            let source = createEmpty<ImageSource>
            source.uri <- Some x.state.uri
            source.isStatic <- Some true
            p.source <- unbox source
            p

        let image =
            createImage 
                imageProps
                // OK this is stupid way to make the image bigger
                [| createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""
                   createText (unbox null) ""  |]

        createView (unbox null)
            [|
                image
                button              
            |]