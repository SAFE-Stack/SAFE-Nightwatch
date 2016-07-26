module HelloWorld

open Fable.Core
open Fable.Import
type RN = ReactNative.Globals

type HelloWorldApp (props) =
    inherit React.Component<obj,obj>(props)

    member x.render () =
        let props =
            createObj [
                "Aspect" ==> "fill"
                "style" ==>
                    createObj [
                        "width" ==> 100
                        "height" ==> 100
                    ]
                ]
        React.createElement(RN.View, unbox null,
            [|
                React.createElement(ReactNativeCamera.Globals.Camera , props) |> unbox
                React.createElement(RN.Text, unbox null, unbox "Hello World") |> unbox
            |]
        )