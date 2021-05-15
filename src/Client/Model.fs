module Model

open System

type LocationId = string

[<RequireQualifiedAccess>]
type LocationStatus =
| Ok
| Alarm of string

type LocationCheckRequest = {
    LocationId : LocationId
    Name: string
    Address: string
    Status : LocationStatus option
    Date : DateTime option
}