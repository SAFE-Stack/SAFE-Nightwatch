module AsyncPromiseExtensions 
open Fable.Core
open Fable.Import

type Microsoft.FSharp.Control.AsyncBuilder with
    member x.Bind(p, f) = 
        async.Bind (Async.AwaitPromise(p), f)