module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared

let locations () =
    [| { LocationId = "X1"
         Name = "Bell tower"
         Address = "Market place" }
       { LocationId = "X2"
         Name = "Graveyard"
         Address = "Right next to church" }
       { LocationId = "X3"
         Name = "Castle black"
         Address = "The wall" }

       |]

let locationApi =
    { getLocation = fun () -> 
        async { 
            printfn "got request"
            return locations () } }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue locationApi
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app