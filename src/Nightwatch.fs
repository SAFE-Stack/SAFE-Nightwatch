module Nightwatch

open Elmish
open Elmish.React
open Elmish.ReactNative

let runnable :obj->obj = 
    Program.mkProgram App.init App.update App.view
    |> Program.withSubscription App.subscribe
    |> Program.withConsoleTrace
    |> Program.toRunnable Program.run