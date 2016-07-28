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

let createText changeProperties text : ReactElement = 
    React.createElement(
        RN.Text, 
        createEmpty<TextProperties> |> changeProperties,
        unbox text) |> unbox

let createTouchableHighlight changeProperties (child:ReactElement) : ReactElement = 
    React.createElement(
        RN.TouchableHighlight, 
        createEmpty<TouchableHighlightProperties> |> changeProperties,
        [| unbox child |]) |> unbox

let createView changeProperties (childs:ReactElement []) : ReactElement = 
    React.createElement(
        RN.View, 
        createEmpty<ViewProperties> |> changeProperties,
        childs |> unbox) |> unbox

let createImage changeProperties (childs:ReactElement []) : ReactElement = 
    React.createElement(
            RN.Image, 
            createEmpty<ImageProperties> |> changeProperties, 
            childs |> unbox) |> unbox 

type ImageSource =
    abstract uri: string option with get, set
    abstract isStatic: bool option with get, set        

let baseUrl = "http://facebook.github.io/react/img/logo_og.png"

type ImagePickerApp (props) as this =
    inherit React.Component<obj,ImagePickerAppState>(props)
    do this.state <- { uri = baseUrl }

    member x.render () =

        let button =
            createText id "click me to select image!"
            |> createTouchableHighlight (fun t ->
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
                t)


        let image =
            createImage 
                (fun p ->
                    let source = createEmpty<ImageSource>
                    source.uri <- Some x.state.uri
                    source.isStatic <- Some true
                    p.source <- unbox source
                    p)
                // OK this is stupid way to make the image bigger
                [| createText id ""
                   createText id ""
                   createText id ""
                   createText id ""
                   createText id ""
                   createText id ""
                   createText id ""
                   createText id ""
                   createText id ""
                   createText id ""
                   createText id ""  |]

        createView id
            [|
                image
                button
            |]