namespace Fable.Import

open System
open Fable.Core
open Fable.Import
open Fable.Import.JS
open Fable.Import.Browser

module ReactImagePicker =

    type ImagePickerManager =
        abstract member showImagePicker: (ImagePickerOptions * (ImagePickerResult -> unit)) -> unit

    and ImagePickerOptions = obj
    and ImagePickerResult = obj

    type Globals =
        [<Import("default", from="react-native-image-picker")>]
        static member ImagePickerManager with get(): ImagePickerManager = failwith "JS only" and set(v: ImagePickerManager): unit = failwith "JS only"