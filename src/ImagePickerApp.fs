module ImagePickerApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

type RN = ReactNative.Globals
type IP = ReactImagePicker.Globals


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
                OnPress 
                    (fun () -> 
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
                                        baseUrl }))]


        let image =
            image 
                [ Source [ Uri x.state.uri; IsStatic true ]]
                // OK this is stupid way to make the image bigger
                [ text [] ""
                  text [] ""
                  text [] ""
                  text [] ""
                  text [] ""
                  text [] ""
                  text [] ""
                  text [] ""
                  text [] ""
                  text [] ""
                  text [] "" ]


        view [] 
         [ image
           button ]