module Nightwatch

open Elmish
open Elmish.React
open Elmish.ReactNative
open Elmish.HMR
open Fable.ReactNative

let setupBackHandler dispatch =
    let backHandler () =
        dispatch App.Msg.NavigateBack
        true

    setOnHardwareBackPressHandler backHandler


let subscribe (model:App.Model) =
    Cmd.batch [
        Cmd.ofSub setupBackHandler ]

Program.mkProgram App.init App.update App.view
|> Program.withSubscription subscribe
#if RELEASE
#else
|> Program.withConsoleTrace
#endif
|> Program.withReactNative "Nightwatch"
|> Program.run