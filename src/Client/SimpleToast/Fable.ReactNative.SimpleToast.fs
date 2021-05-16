module Fable.ReactNative.SimpleToast

open Fable.Core
open Fable.Core.JsInterop
    
[<RequireQualifiedAccess>]  
module Toaster =
    [<Import("Toast","react-native-simple-toast")>]
    let private Toast = obj()

    /// Shows a toast with short duration
    let showShort (message:string) : unit =
        !!Toast?show(message,Toast?SHORT)

    /// Shows a toast with long duration
    let showLong (message:string) : unit =
        !!Toast?show(message,Toast?LONG)