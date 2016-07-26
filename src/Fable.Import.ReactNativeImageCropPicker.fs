namespace Fable.Import

open System
open Fable.Core
open Fable.Import
open Fable.Import.JS
open Fable.Import.Browser

module ReactImageCropPicker =

    type CameraStatic =
        inherit React.ComponentClass<CameraProperties>

    and CameraProperties = obj

    type Globals =
        [<Import("default", from="react-native-image-crop-picker")>]
        static member ImagePicker with get(): CameraStatic = failwith "JS only" and set(v: CameraStatic): unit = failwith "JS only"