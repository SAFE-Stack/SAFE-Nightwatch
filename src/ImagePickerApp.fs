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

type ImagePickerApp (props) =
    inherit React.Component<obj,obj>(props)

    member x.render () =
        let routeStack = [| createRoute("Main",0); createRoute("ImagePicker",1) |]
        navigator [
            InitialRoute routeStack.[0]
            InitialRouteStack routeStack
            RenderScene (Func<_,_,_>(fun route navigator ->
                match route.title with
                | "ImagePicker" ->
                    createScene<ImagePickerScene.ImagePickerScene,_,_>(
                        {
                            initState = { uri = "http://facebook.github.io/react/img/logo_og.png" }
                            onDone = 
                                (fun imagePickerState ->
                                    if route.index > 0 then
                                        navigator.pop()
                                )                        
                        })
                | _ ->
                    createScene<MainScene.MainScene,_,_>(
                        {
                            onGoToImagePicker = (fun () -> navigator.push routeStack.[1])
                        })
            ))
            
        ]

        
