module Model

open System

type LocationId = string

[<RequireQualifiedAccess>]
type LocationStatus =
| NotChecked
| Ok
| Alarm of string

type LocationCheckRequest = {
    LocationId : LocationId
    Name: string
    Address: string    
    Status : LocationStatus
    Date : DateTime option
}