module Messages

open System

[<RequireQualifiedAccess>]
type Page =
| Home
| LocationList
| CheckLocation of int * Model.LocationCheckRequest

type AppMsg =
| PushNotificationClicked of string * Fable.Import.ReactNativeOneSignal.OneSignalNotificationData * bool
| NavigateTo of Page
| SetDeviceID of Fable.Import.ReactNativeOneSignal.OneSignalID
| SendAlarm of string
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