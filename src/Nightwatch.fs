module internal Nightwatch

open System
open Fable.Import
open Elmish
open Messages
open App

type Nightwatch (props) as this =
    inherit React.Component<obj,AppModel>(props)

    let safeState state = 
        match !loaded with 
        | false -> this.state <- state
        | _ -> this.setState state

    let dispatch = program |> Program.run safeState 
    let backHandler = (fun () -> dispatch AppMsg.NavigateBack; true)
        
    member x.componentDidMount() =
        if not !loaded then
            Fable.Helpers.ReactNative.setOnHardwareBackPressHandler backHandler

        loaded := true
        Database.createDemoData() // Make sure we have some data

    member x.componentWillUnmount() =  
        if !loaded then
            Fable.Helpers.ReactNative.removeOnHardwareBackPressHandler backHandler

        loaded := false

    member x.render () =
        if !loaded then
            view this.state dispatch
        else
            view (init() |> fst) dispatch 