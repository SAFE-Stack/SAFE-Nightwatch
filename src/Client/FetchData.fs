module FetchData

open Shared
open Fable.Remoting.Client

let locationApi = 
    Remoting.createApi ()
    |> Remoting.withBaseUrl "http://0.0.0.0:8085"
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ILocationApi> 