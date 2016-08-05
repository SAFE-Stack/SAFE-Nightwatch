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
        navigator [
            InitialRoute (createRoute("Main",0))
            RenderScene (Func<_,_,_>(fun route navigator ->
                match route.id with
                | "ImagePicker" ->
                    createScene<ImagePickerScene.ImagePickerScene,_,_>(
                        {
                            initUri = "http://facebook.github.io/react/img/logo_og.png"
                            Navigator = navigator                    
                        })
                | _ ->
                    createScene<MainScene.MainScene,_,_>(
                        {
                            Navigator = navigator
                        })
            ))
        ]

        
