module HelloWorld

open Fable.Core
open Fable.Import
type RN = ReactNative.Globals


type HelloWorldApp (props) =
    inherit React.Component<obj,obj>(props)

    member x.render ()=
        React?createElement(RN.Text , null, "Hello World")