import { Record, Union } from "../.fable/fable-library.3.0.0/Types.js";
import { record_type, class_type, array_type, tuple_type, int32_type, union_type, string_type } from "../.fable/fable-library.3.0.0/Reflection.js";
import { LocationCheckRequest$reflection } from "../Model.js";
import { Cmd_OfPromise_either, Cmd_none, Cmd_OfFunc_result } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { getIndexedCheckRequests } from "../Database.js";
import { printf, toText } from "../.fable/fable-library.3.0.0/String.js";
import * as react from "react";
import { FlatList, Image, Text as Text$, View, TouchableHighlight } from "react-native";
import { Props_FlatListProperties$1, Props_ImageProperties_Style_7FB9FF3D, Props_ImageProperties, Props_FlexStyle, Props_ViewProperties_Style_7FB9FF3D } from "../.fable/Fable.React.Native.3.0.0-alpha001/Fable.ReactNative.fs.js";
import { filter, singleton, ofArray } from "../.fable/fable-library.3.0.0/List.js";
import { keyValueList } from "../.fable/fable-library.3.0.0/MapUtil.js";
import { button, titleText, sceneBackground, defaultText } from "../Styles.js";
import { int32ToString, uncurry } from "../.fable/fable-library.3.0.0/Util.js";

export class Status extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["NotStarted", "InProgress", "Complete"];
    }
}

export function Status$reflection() {
    return union_type("LocationList.Status", [], Status, () => [[], [], [["Item", string_type]]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["CheckNextLocation", "GoBack", "RefreshList", "NewLocationCheckRequests", "Error"];
    }
}

export function Msg$reflection() {
    return union_type("LocationList.Msg", [], Msg, () => [[["Item1", int32_type], ["Item2", LocationCheckRequest$reflection()]], [], [], [["Item", array_type(tuple_type(int32_type, LocationCheckRequest$reflection()))]], [["Item", class_type("System.Exception")]]]);
}

export class Model extends Record {
    constructor(Requests, Status) {
        super();
        this.Requests = Requests;
        this.Status = Status;
    }
}

export function Model$reflection() {
    return record_type("LocationList.Model", [], Model, () => [["Requests", array_type(tuple_type(int32_type, LocationCheckRequest$reflection()))], ["Status", Status$reflection()]]);
}

export function init() {
    return [new Model([], new Status(0)), Cmd_OfFunc_result(new Msg(2))];
}

export function update(msg, model) {
    let arg10;
    switch (msg.tag) {
        case 0: {
            return [model, Cmd_none()];
        }
        case 2: {
            return [new Model(model.Requests, new Status(1)), Cmd_OfPromise_either(getIndexedCheckRequests, void 0, (arg0) => (new Msg(3, arg0)), (arg0_1) => (new Msg(4, arg0_1)))];
        }
        case 3: {
            const indexedRequests = msg.fields[0];
            return [new Model(indexedRequests, new Status(2, (arg10 = (indexedRequests.length | 0), toText(printf("Locations: %d"))(arg10)))), Cmd_none()];
        }
        case 4: {
            const e = msg.fields[0];
            return [new Model(model.Requests, new Status(2, e.message)), Cmd_none()];
        }
        default: {
            return [model, Cmd_none()];
        }
    }
}

export function view(model, dispatch) {
    let props_13, props_15, matchValue_1, s_4, props_17, pascalCaseProps;
    const renderItem = (tupledArg) => {
        let props_8, props, props_2, matchValue, status, uri, text_3, props_6;
        const pos = tupledArg[0] | 0;
        const request = tupledArg[1];
        return react.createElement(TouchableHighlight, {
            onPress: () => {
                dispatch(new Msg(0, pos, request));
            },
        }, (props_8 = singleton(Props_ViewProperties_Style_7FB9FF3D(ofArray([new Props_FlexStyle(21, "center"), new Props_FlexStyle(1, "center"), new Props_FlexStyle(14, 1), new Props_FlexStyle(16, "row")]))), react.createElement(View, keyValueList(props_8, 1), (props = singleton(defaultText()), react.createElement(Text$, keyValueList(props, 1), request.Name)), (props_2 = singleton(defaultText()), react.createElement(Text$, keyValueList(props_2, 1), request.Address)), (matchValue = request.Status, (matchValue != null) ? (status = matchValue, (uri = ((status.tag === 1) ? (text_3 = status.fields[0], require("${entryDir}/../images/Alarm.png")) : (require("${entryDir}/../images/Approve.png"))), (props_6 = ofArray([new Props_ImageProperties(8, uri), Props_ImageProperties_Style_7FB9FF3D(ofArray([new Props_FlexStyle(50, 24), new Props_FlexStyle(20, 24), new Props_FlexStyle(2, "center")]))]), react.createElement(Image, keyValueList(props_6, 1))))) : react.createElement(Text$, {}, "")))));
    };
    const props_19 = singleton(sceneBackground());
    return react.createElement(View, keyValueList(props_19, 1), (props_13 = singleton(titleText()), react.createElement(Text$, keyValueList(props_13, 1), "Locations to check")), (props_15 = singleton(defaultText()), react.createElement(Text$, keyValueList(props_15, 1), (matchValue_1 = model.Status, (matchValue_1.tag === 2) ? (s_4 = matchValue_1.fields[0], s_4) : ""))), (props_17 = ofArray([new Props_FlatListProperties$1(8, 20), new Props_FlatListProperties$1(10, uncurry(2, (tupledArg_1) => {
        const i = tupledArg_1[0] | 0;
        return (_arg1) => int32ToString(i);
    })), new Props_FlatListProperties$1(19, (v) => renderItem(v.item))]), (pascalCaseProps = filter((_arg1_1) => ((_arg1_1.tag === 0) ? true : ((_arg1_1.tag === 1) ? true : ((_arg1_1.tag === 2) ? true : ((_arg1_1.tag === 3) ? true : false)))), props_17), react.createElement(FlatList, Object.assign({
        data: model.Requests,
    }, keyValueList(props_17, 1), keyValueList(pascalCaseProps, 0))))), button("OK", () => {
        dispatch(new Msg(1));
    }));
}

