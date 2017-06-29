module Nightwatch

open Elmish
open Elmish.React
open Elmish.ReactNative

Program.mkProgram App.init App.update App.view
|> Program.withSubscription App.subscribe
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactNative "nightwatch"
|> Program.run