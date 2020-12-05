import { Record, Union } from "../.fable/fable-library.3.0.0/Types.js";
import { record_type, string_type, union_type, class_type, int32_type } from "../.fable/fable-library.3.0.0/Reflection.js";
import { printf, toText } from "../.fable/fable-library.3.0.0/String.js";
import { Cmd_OfFunc_result, Cmd_OfPromise_either, Cmd_none } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { createDemoData } from "../Database.js";
import { Props_FlexStyle, Props_ImageProperties_Style_7FB9FF3D, Props_ImageProperties } from "../.fable/Fable.React.Native.3.0.0-alpha001/Fable.ReactNative.fs.js";
import { singleton, ofArray } from "../.fable/fable-library.3.0.0/List.js";
import * as react from "react";
import { Text as Text$, View, Image } from "react-native";
import { keyValueList } from "../.fable/fable-library.3.0.0/MapUtil.js";
import { smallText, whitespace, button, titleText, sceneBackground } from "../Styles.js";

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["GetDemoData", "NewDemoData", "BeginWatch", "Error"];
    }
}

export function Msg$reflection() {
    return union_type("Home.Msg", [], Msg, () => [[], [["Item", int32_type]], [], [["Item", class_type("System.Exception")]]]);
}

export class Model extends Record {
    constructor(StatusText) {
        super();
        this.StatusText = StatusText;
    }
}

export function Model$reflection() {
    return record_type("Home.Model", [], Model, () => [["StatusText", string_type]]);
}

export function update(msg, model) {
    switch (msg.tag) {
        case 1: {
            const count = msg.fields[0] | 0;
            return [new Model(toText(printf("Locations: %d"))(count)), Cmd_none()];
        }
        case 2: {
            return [model, Cmd_none()];
        }
        case 3: {
            const e = msg.fields[0];
            return [new Model(e.message), Cmd_none()];
        }
        default: {
            return [new Model("Syncing..."), Cmd_OfPromise_either(createDemoData, void 0, (arg0) => (new Msg(1, arg0)), (arg0_1) => (new Msg(3, arg0_1)))];
        }
    }
}

export function init() {
    return [new Model(""), Cmd_OfFunc_result(new Msg(0))];
}

export function view(model, dispatch) {
    let props_2, props_4;
    let logo;
    const props = ofArray([new Props_ImageProperties(8, require("${entryDir}/../images/raven.jpg")), Props_ImageProperties_Style_7FB9FF3D(ofArray([new Props_FlexStyle(2, "center"), new Props_FlexStyle(16, "column")]))]);
    logo = react.createElement(Image, keyValueList(props, 1));
    const props_6 = singleton(sceneBackground());
    return react.createElement(View, keyValueList(props_6, 1), (props_2 = singleton(titleText()), react.createElement(Text$, keyValueList(props_2, 1), "Nightwatch")), logo, button("Begin watch", () => {
        dispatch(new Msg(2));
    }), whitespace(), whitespace(), (props_4 = singleton(smallText()), react.createElement(Text$, keyValueList(props_4, 1), model.StatusText)));
}

