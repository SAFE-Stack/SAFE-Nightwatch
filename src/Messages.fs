module Messages

open System

[<RequireQualifiedAccess>]
type Page =
| Home
| LocationList
| CheckLocation of int * Model.LocationCheckRequest

type AppMsg =
| NavigateTo of Page
| NavigateBack
| ExitApp
| HomeSceneMsg of HomeSceneMsg
| LocationListMsg of LocationListMsg
| LocationCheckMsg of LocationCheckMsg

and [<RequireQualifiedAccess>] HomeSceneMsg =
| GetDemoData
| NewDemoData of int
| BeginWatch
| Error of exn

and [<RequireQualifiedAccess>] LocationListMsg =
| CheckNextLocation of int * Model.LocationCheckRequest
| RefreshList
| NewLocationCheckRequests of (int * Model.LocationCheckRequest)[]
| Error of exn

and [<RequireQualifiedAccess>] LocationCheckMsg =
| PictureSelected of string option
| LocationStatusUpdated of Model.LocationStatus
| SelectPicture
| SaveAndGoBack
| GoBack
| Error of exn


module Routes =
    [<Literal>] 
    let Main = "main"
    [<Literal>] 
    let Current = "current"
    [<Literal>] 
    let Retrieved = "retrieved"