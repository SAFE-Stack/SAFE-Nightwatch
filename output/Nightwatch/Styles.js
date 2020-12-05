import { Props_ViewStyle, Props_ViewProperties_Style_7FB9FF3D, Props_FlexStyle, Props_TextStyle, Props_TextProperties_Style_7FB9FF3D } from "./.fable/Fable.React.Native.3.0.0-alpha001/Fable.ReactNative.fs.js";
import { singleton, ofArray } from "./.fable/fable-library.3.0.0/List.js";
import * as react from "react";
import { Button, Text as Text$ } from "react-native";
import { keyValueList } from "./.fable/fable-library.3.0.0/MapUtil.js";

export function renderText(fontSize) {
    return Props_TextProperties_Style_7FB9FF3D(ofArray([new Props_TextStyle(0, "#FFFFFF"), new Props_TextStyle(7, "center"), new Props_FlexStyle(23, 3), new Props_TextStyle(2, fontSize)]));
}

export function defaultText() {
    return renderText(15);
}

export function mediumText() {
    return renderText(12);
}

export function smallText() {
    return renderText(10);
}

export function titleText() {
    return renderText(17);
}

export function whitespace() {
    const props = singleton(smallText());
    return react.createElement(Text$, keyValueList(props, 1), "");
}

export function sceneBackground() {
    return Props_ViewProperties_Style_7FB9FF3D(ofArray([new Props_FlexStyle(2, "stretch"), new Props_FlexStyle(37, 20), new Props_ViewStyle(25, "#000000"), new Props_ViewStyle(27, 0.8), new Props_ViewStyle(28, 3), new Props_FlexStyle(21, "center"), new Props_FlexStyle(14, 1), new Props_ViewStyle(1, "#615A5B")]));
}

export function button(label, onPress) {
    return react.createElement(Button, {
        title: label,
        onPress: onPress,
    });
}

