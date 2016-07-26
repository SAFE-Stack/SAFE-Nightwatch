namespace Fable.Import

open System
open Fable.Core
open Fable.Import
open Fable.Import.JS
open Fable.Import.Browser

module ReactNativeCamera =

    type CameraStatic =
        inherit React.ComponentClass<CameraProperties>

    and CameraProperties = obj

    type Globals =
        [<Import("default", from="react-native-camera")>]
        static member Camera with get(): CameraStatic = failwith "JS only" and set(v: CameraStatic): unit = failwith "JS only"