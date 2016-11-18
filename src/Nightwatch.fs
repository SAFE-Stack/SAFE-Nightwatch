module Nightwatch

open System
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Elmish
open Elmish.React
open Elmish.ReactNative
open Messages
    
let setupBackHandler dispatch =
    let backHandler () =
        dispatch AppMsg.NavigateBack
        true

    Fable.Helpers.ReactNative.setOnHardwareBackPressHandler backHandler

let subscribe _ =
    Cmd.ofSub setupBackHandler

let runnable :obj->obj = 
    Program.mkProgram App.init App.update App.view
    |> Program.withSubscription subscribe
    |> Program.withConsoleTrace
    |> Program.toRunnable Program.run