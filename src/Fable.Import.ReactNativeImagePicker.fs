namespace Fable.Import

open System
open Fable.Core
open Fable.Import
open Fable.Import.JS
open Fable.Import.Browser

module ReactImagePicker =

    type ImagePicker =
        abstract member showImagePicker: ImagePickerOptions -> Promise<ImagePickerResult>

    and ImagePickerOptions = obj
    and ImagePickerResult = obj

    type Globals =
        [<Import("default", from="react-native-image-picker")>]
        static member ImagePicker with get(): ImagePicker = failwith "JS only" and set(v: ImagePicker): unit = failwith "JS only"