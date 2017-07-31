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
open Fable.Core.JsInterop

[<RequireQualifiedAccess>]
type Page =
| Home
| LocationList
| CheckLocation of int * Model.LocationCheckRequest

type Msg =
| PushNotificationClicked of string * Fable.Import.ReactNativeOneSignal.OneSignalNotificationData * bool
| NavigateTo of Page
| SetDeviceID of Fable.Import.ReactNativeOneSignal.OneSignalID
| SendAlarm of string
| NavigateBack
| ExitApp
| HomeSceneMsg of Home.Msg
| LocationListMsg of LocationList.Msg
| CheckLocationMsg of CheckLocation.Msg

type SubModel =
| HomeModel of Home.Model
| LocationListModel of LocationList.Model
| CheckLocationModel of CheckLocation.Model

type Model = {
    OneSignalID : ReactNativeOneSignal.OneSignalID option
    SubModel : SubModel
    NavigationStack: Page list
}

let wrap ctor msgCtor model (subModel,cmd)  =
    { model with SubModel = ctor subModel }, Cmd.map msgCtor cmd

let navigateTo page newStack model =
    match page with
    | Page.LocationList -> LocationList.init() |> wrap LocationListModel LocationListMsg model
    | Page.CheckLocation (pos,request) -> CheckLocation.init(pos,request) |> wrap CheckLocationModel CheckLocationMsg model
    | Page.Home -> Home.init() |> wrap HomeModel HomeSceneMsg model
    |> fun (model,cmd) -> { model with NavigationStack = newStack },cmd

let update (msg:Msg) model : Model*Cmd<Msg> = 
    match msg with
    | HomeSceneMsg subMsg ->
        match model.SubModel with
        | HomeModel subModel -> 
            match subMsg with
            | Home.Msg.BeginWatch ->
                model, Cmd.ofMsg (NavigateTo Page.LocationList)
            | _ ->
                Home.update subMsg subModel |> wrap HomeModel HomeSceneMsg model
        | _ -> model,Cmd.none

    | LocationListMsg subMsg ->
        match model.SubModel with
        | LocationListModel subModel -> 
            match subMsg with
            | LocationList.Msg.GoBack -> 
                model, Cmd.ofMsg NavigateBack
            | LocationList.Msg.CheckNextLocation(pos,request) ->
                model, Cmd.ofMsg (NavigateTo (Page.CheckLocation(pos,request)))
            | _ ->
                LocationList.update subMsg subModel |> wrap LocationListModel LocationListMsg model
        | _ -> model,Cmd.none

    | CheckLocationMsg subMsg ->
        match model.SubModel with
        | CheckLocationModel subModel -> 
            match subMsg with
            | CheckLocation.Msg.SendAlarm alarm -> 
                model, Cmd.ofMsg (SendAlarm alarm)
            | CheckLocation.Msg.GoBack -> 
                model, Cmd.ofMsg NavigateBack
            | _ ->
                CheckLocation.update subMsg subModel |> wrap CheckLocationModel CheckLocationMsg model
        | _ -> model,Cmd.none

    | NavigateTo page -> 
        navigateTo page (page::model.NavigationStack) model

    | PushNotificationClicked(message,data,isActive) -> 
        model,Cmd.none

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
      NavigationStack = [Page.Home] }, Cmd.map HomeSceneMsg cmd

let view (model:Model) (dispatch: Msg -> unit) =
    match model.SubModel with
    | HomeModel model -> lazyView2 Home.view model (HomeSceneMsg >> dispatch)
    | CheckLocationModel model -> lazyView2 CheckLocation.view model (CheckLocationMsg >> dispatch)
    | LocationListModel model -> lazyView2 LocationList.view model (LocationListMsg >> dispatch)

