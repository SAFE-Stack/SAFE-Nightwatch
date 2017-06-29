module App

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Import.ReactNativeOneSignal
open Fable.Helpers.ReactNativeOneSignal
open Fable.Helpers.ReactNativeOneSignal.Props
open Elmish
open Elmish.React
open Elmish.ReactNative
open Messages
open Fable.Core.JsInterop

type SubModel =
| HomeModel of Home.Model
| LocationListModel of LocationList.Model
| CheckLocationModel of CheckLocation.Model

type AppModel = {
    OneSignalID : ReactNativeOneSignal.OneSignalID option
    SubModel : SubModel
    NavigationStack: Page list
}

let wrap ctor model (subModel,cmd)  =
    { model with SubModel = ctor subModel },cmd

let navigateTo page newStack model =
    match page with
    | Page.LocationList -> LocationList.init() |> wrap LocationListModel model
    | Page.CheckLocation (pos,request) -> CheckLocation.init(pos,request) |> wrap CheckLocationModel model
    | Page.Home -> Home.init() |> wrap HomeModel model
    |> fun (model,cmd) -> { model with NavigationStack = newStack },cmd

let update (msg:AppMsg) model : AppModel*Cmd<AppMsg> = 
    match msg with
    | HomeSceneMsg subMsg ->
        match model.SubModel with
        | HomeModel subModel -> Home.update subMsg subModel |> wrap HomeModel model
        | _ -> model,Cmd.none
    | LocationListMsg subMsg ->
        match model.SubModel with
        | LocationListModel subModel -> LocationList.update subMsg subModel |> wrap LocationListModel model
        | _ -> model,Cmd.none
    | LocationCheckMsg subMsg ->
        match model.SubModel with
        | CheckLocationModel subModel -> CheckLocation.update subMsg subModel |> wrap CheckLocationModel model
        | _ -> model,Cmd.none
    | NavigateTo page -> navigateTo page (page::model.NavigationStack) model
    | PushNotificationClicked(message,data,isActive) -> model,Cmd.none
    | SetDeviceID id -> 
        { model with OneSignalID = Some id }, Cmd.none
    | NavigateBack -> 
        match model.NavigationStack with
        | _::page::rest -> navigateTo page (page::rest) model
        | _ -> model,Cmd.ofMsg ExitApp
    | SendAlarm message ->
        match model.OneSignalID with
        | None -> ()
        | Some device ->
            let contents = 
                createObj [
                    "en" ==> message
                ] |> unbox
            let data = [||]
            OneSignal.postP2PNotification(contents,data,device.userId)
        model, Cmd.none
    | ExitApp -> 
        Fable.Helpers.ReactNative.exitApp() 
        model,Cmd.none

let init() =
    let subModel,cmd = Home.init() 
    { SubModel = HomeModel subModel
      OneSignalID = None
      NavigationStack = [Page.Home] }, cmd

let view (model:AppModel) (dispatch: AppMsg -> unit) =
    match model.SubModel with
    | HomeModel model -> lazyView2 Home.view model dispatch
    | CheckLocationModel model -> lazyView2 CheckLocation.view model dispatch
    | LocationListModel model -> lazyView2 LocationList.view model dispatch


let setuOneSignal dispatch =
    OneSignal.configure 
        [OnNotificationOpened (Func<_,_,_,_>(fun message data isActive -> dispatch (AppMsg.PushNotificationClicked(message,data,isActive))))
         OnIdsAvailable (AppMsg.SetDeviceID >> dispatch)]
    
    OneSignal.enableInAppAlertNotification()
    OneSignal.enableVibrate()
    OneSignal.enableSound()
    OneSignal.enableNotificationsWhenActive()


let setupBackHandler dispatch =    
    let backHandler () =
        dispatch AppMsg.NavigateBack
        true

    Fable.Helpers.ReactNative.setOnHardwareBackPressHandler backHandler

let subscribe (model:AppModel) =
    Cmd.batch [
        Cmd.ofSub setupBackHandler
        Cmd.ofSub setuOneSignal]