module internal Styles

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

let brandPrimary = "#428bca"
let brandInfo = "#5bc0de"
let brandSuccess = "#5cb85c"
let brandDanger = "#d9534f"
let brandWarning = "#f0ad4e"
let brandSidebar = "#252932"

let inverseTextColor = "#000"
let textColor = "#fff"
let shadowColor = "#000000"
let backgroundColor = "#615A5B"

let touched = "#5499C4"

let fontSizeBase = 15.
let titleFontSize = 17.

let borderRadius = 4.

let buttonStyle =
    TouchableHighlightProperties.Style [
        ViewStyle.BackgroundColor brandPrimary
        ViewStyle.BorderRadius borderRadius
        ViewStyle.MarginBottom 5.
      ]

let defaultText =
    TextProperties.Style [ 
        TextStyle.Color textColor
        TextStyle.TextAlign TextAlignment.Center
        TextStyle.MarginBottom 5.
        TextStyle.FontSize fontSizeBase
      ]

let titleText =
    TextProperties.Style [ 
        TextStyle.Color textColor
        TextStyle.TextAlign TextAlignment.Center
        TextStyle.MarginBottom 15.
        TextStyle.FontSize titleFontSize
      ] 
      

let sceneBackground =
    ViewProperties.Style [ 
        ViewStyle.AlignSelf Alignment.Stretch
        ViewStyle.Padding 20.
        ViewStyle.ShadowColor shadowColor
        ViewStyle.ShadowOpacity 0.8
        ViewStyle.ShadowRadius 3.
        ViewStyle.JustifyContent Alignment.Center
        ViewStyle.Flex 1
        ViewStyle.BackgroundColor backgroundColor
      ]    