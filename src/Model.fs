module Model

open System

type LocationId = string

type LocationCheckRequest = {
    LocationId : LocationId
    Name: string
    Address: string
} 

[<RequireQualifiedAccess>]
type LocationStatus =
| Ok
| Alarm of string

type LocationCheckResult = {
    LocationId : LocationId
    Status : LocationStatus
    Date : DateTime
    PictureUri : string option
}
    with
        static member FromRequest(request:LocationCheckRequest) = {
            LocationId = request.LocationId
            Status = LocationStatus.Ok
            Date = DateTime.Now
            PictureUri = None
        }