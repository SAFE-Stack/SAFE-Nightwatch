module Nightwatch

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactNativeImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore


type Nightwatch (props) =
    inherit React.Component<obj,obj>(props)

    member x.componentDidMount() = () //Database.createDemoData() // Make sure we have some data

    member x.render () =
        Styles.whitespace
        // navigator [
        //     InitialRoute (createRoute("Start",0))
        //     RenderScene (Func<_,_,_>(fun route navigator ->
        //         match route.id with
        //         | "CheckLocation" ->
        //             createScene<CheckLocationScene.CheckLocationScene,_,_>(
        //                 {
        //                     ReadingRequest = unbox route.payload
        //                     Route = route
        //                     Navigator = navigator
        //                 })
        //         | "LocationListScene" ->
        //             createScene<LocationListScene.LocationListScene,_,_>(
        //                 {
        //                     Navigator = navigator
        //                 })                        
        //         | _ ->
        //             createScene<MainScene.MainScene,_,_>(
        //                 {
        //                     Navigator = navigator
        //                 }) 
        //     ))
            
        // ]

        
