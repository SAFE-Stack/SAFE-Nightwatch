module Nightwatch

open System
open Elmish
open Elmish.React
open Elmish.ReactNative
open Elmish.HMR
open Fable.Core.JsInterop

let setupBackHandler dispatch =    
    let backHandler () =
        dispatch App.Msg.NavigateBack
        true

    Fable.Helpers.ReactNative.setOnHardwareBackPressHandler backHandler


let subscribe (model:App.Model) =
    Cmd.batch [
        Cmd.ofSub setupBackHandler ]


Program.mkProgram App.init App.update App.view
|> Program.withSubscription subscribe
#if RELEASE
#else
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReactNative "nightwatch"
|> Program.run