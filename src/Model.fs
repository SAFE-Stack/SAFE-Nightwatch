module Model

open System
open Fable.Core

type LocationId = string

[<RequireQualifiedAccess>]
type LocationStatus =
| Ok
| Alarm of string

type [<Pojo>] LocationCheckRequest = {
    LocationId : LocationId
    Name: string
    Address: string
    Status : LocationStatus option
    Date : DateTime option
}