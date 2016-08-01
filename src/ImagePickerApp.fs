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

type RN = ReactNative.Globals

type ImagePickerApp (props) =
    inherit React.Component<obj,obj>(props)

    member x.render () =
        navigator [
            InitialRoute (createRoute("Main",0))
            RenderScene (Func<_,_,_>(fun route navigator -> 
                match route.index with
                | 1 ->
                    createScene<ImagePickerScene.ImagePickerScene,_,_>(
                        {
                            title = route.title
                            onDone = 
                                (fun () ->
                                    if route.index > 0 then
                                        navigator.pop()
                                )                        
                        })
                | _ ->                        
                    createScene<MainScene.MainScene,_,_>(
                        {
                            title = route.title
                            onGoToImagePicker = 
                                (fun () ->
                                    let nextIndex = route.index + 1 
                                    createRoute("Scene " + nextIndex.ToString(),nextIndex)
                                    |> navigator.push
                                )                  
                        }))
            )
        ]
