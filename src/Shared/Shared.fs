namespace Shared

type LocationCheckRequest = 
    { LocationId: string; Name: string; Address: string  }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ILocationApi =
    { getLocation: unit -> Async<LocationCheckRequest []> }