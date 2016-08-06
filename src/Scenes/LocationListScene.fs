module internal LocationListScene

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactNativeImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore
open Fable.Core.JsInterop

type LocationListSceneProperties = {
    Navigator: Navigator
}

type LocationListSceneState = {
    StatusText : string
    RequestDataSource : ListViewDataSource<Model.LocationCheckRequest * Model.LocationCheckResult option>
}

type LocationListScene (props) as this =
    inherit React.Component<LocationListSceneProperties,LocationListSceneState>(props)

    do this.state <- { 
        RequestDataSource = emptyDataSource() 
        StatusText = "" }
    
    member x.componentDidMount() = x.RefreshData()

    member x.RefreshData() =
        async { 
            try
                let! requests = DB.getAll<Model.LocationCheckRequest>()
                let! results = DB.getAll<Model.LocationCheckResult>()

                let model =
                    requests
                    |> Array.map (fun request -> request,results |> Array.tryFind (fun result -> result.LocationId = request.LocationId))
                
                x.setState { x.state with RequestDataSource = updateDataSource model x.state.RequestDataSource }
            with
            | error -> x.setState { x.state with StatusText = error.Message }
        } 
        |> Async.StartImmediate

    member x.render () =
        view [ Styles.sceneBackground ] 
            [ text [ Styles.titleText ] "Locations to check"
              text [ Styles.defaultText ] x.state.StatusText
              listView 
                x.state.RequestDataSource
                [ ListViewProperties.RenderRow  
                    (Func<_,_,_,_,_>(fun (request,result) b c d ->
                        view [
                            ViewProperties.Style[
                                ViewStyle.JustifyContent Alignment.Center
                                ViewStyle.AlignItems ItemAlignment.Center
                                ViewStyle.Flex 1
                                ViewStyle.FlexDirection FlexDirection.Row ]]
                            [ text [ Styles.defaultText ] request.LocationId
                              text [ Styles.defaultText ] request.Name
                              text [ Styles.defaultText ] request.Address
                              (if result = None then 
                                text [] ""
                              else 
                                image
                                    [ Source (localImage "../../images/Approve.png")
                                      ImageProperties.Style [
                                        ImageStyle.Width 24.
                                        ImageStyle.Height 24.
                                        ImageStyle.AlignSelf Alignment.Center
                                      ]
                                    ])
                             ]
                        |> touchableHighlight [OnPress (fun () -> createFullRoute("CheckLocation",2,request,x.RefreshData,id) |> x.props.Navigator.push)]))
                ]
              Styles.button "OK" x.props.Navigator.pop
            ]
