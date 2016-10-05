module internal App

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Elmish
open Messages

type SubModel =
| HomeModel of Home.Model
| LocationListModel of LocationList.Model
| CheckLocationModel of CheckLocation.Model

type AppModel = {
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
    | NavigateBack -> 
        match model.NavigationStack with
        | _::page::rest -> navigateTo page (page::rest) model
        | _ -> model,Cmd.ofMsg ExitApp
    | ExitApp -> 
        Fable.Helpers.ReactNative.exitApp() 
        model,Cmd.none

let init() =
    let subModel,cmd = Home.init() 
    { SubModel = HomeModel subModel
      NavigationStack = [Page.Home] }, cmd

// App
let program =
    Program.mkProgram init update
    |> Program.withConsoleTrace

let loaded = ref false

let view (model:AppModel) (dispatch: AppMsg -> unit) =
    match model.SubModel with
    | HomeModel model -> Home.view model dispatch
    | CheckLocationModel model -> CheckLocation.view model dispatch
    | LocationListModel model -> LocationList.view model dispatch