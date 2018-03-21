module App

open Elmish
open Elmish.React

[<RequireQualifiedAccess>]
type Page =
| Home
| LocationList
| CheckLocation of int * Model.LocationCheckRequest

type Msg =
| NavigateTo of Page
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
            | CheckLocation.Msg.GoBack -> 
                model, Cmd.ofMsg NavigateBack
            | _ ->
                CheckLocation.update subMsg subModel |> wrap CheckLocationModel CheckLocationMsg model
        | _ -> model,Cmd.none

    | NavigateTo page -> 
        navigateTo page (page::model.NavigationStack) model

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
      NavigationStack = [Page.Home] }, Cmd.map HomeSceneMsg cmd

let view (model:Model) (dispatch: Msg -> unit) =
    match model.SubModel with
    | HomeModel model -> Home.view model (HomeSceneMsg >> dispatch)
    | CheckLocationModel model -> CheckLocation.view model (CheckLocationMsg >> dispatch)
    | LocationListModel model -> LocationList.view model (LocationListMsg >> dispatch)
