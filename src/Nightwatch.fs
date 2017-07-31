module Nightwatch

open System
open Elmish
open Elmish.React
open Elmish.ReactNative

open Fable.Import.ReactNativeOneSignal
open Fable.Helpers.ReactNativeOneSignal
open Fable.Helpers.ReactNativeOneSignal.Props
open Fable.Core.JsInterop

let setupBackHandler dispatch =    
    let backHandler () =
        dispatch App.Msg.NavigateBack
        true

    Fable.Helpers.ReactNative.setOnHardwareBackPressHandler backHandler

let setuOneSignal dispatch =
    OneSignal.configure 
        [OnNotificationOpened (Func<_,_,_,_>(fun message data isActive -> dispatch (App.Msg.PushNotificationClicked(message,data,isActive))))
         OnIdsAvailable (App.Msg.SetDeviceID >> dispatch)]
    
    OneSignal.enableInAppAlertNotification()
    OneSignal.enableVibrate()
    OneSignal.enableSound()
    OneSignal.enableNotificationsWhenActive()



let subscribe (model:App.Model) =
    Cmd.batch [
        Cmd.ofSub setupBackHandler
        Cmd.ofSub setuOneSignal]

Program.mkProgram App.init App.update App.view
|> Program.withSubscription subscribe
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactNative "nightwatch"
|> Program.run