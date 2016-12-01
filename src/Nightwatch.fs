module Nightwatch

open Elmish
open Elmish.React
open Elmish.ReactNative

Program.mkProgram App.init App.update App.view
|> Program.withSubscription App.subscribe
|> Program.withConsoleTrace
|> Program.withReactNative "nightwatch"
|> Program.run