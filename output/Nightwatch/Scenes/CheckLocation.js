import { Record, Union } from "../.fable/fable-library.3.0.0/Types.js";
import { array_type, record_type, int32_type, class_type, option_type, union_type, string_type } from "../.fable/fable-library.3.0.0/Reflection.js";
import { LocationStatus, LocationCheckRequest as LocationCheckRequest_1, LocationCheckRequest$reflection, LocationStatus$reflection } from "../Model.js";
import { Cmd_OfFunc_result, Cmd_OfPromise_either, Cmd_none } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { AsyncStorage } from "../SimpleStore/Fable.ReactNative.SimpleStore.DB.js";
import { Auto_unsafeFromString_Z5CB6BD } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { Auto_toString_5A41365E } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { Props_ImagePickerOptions, Helpers_showImagePickerAsync } from "../.fable/Fable.React.Native.3.0.0-alpha001/extra/react-native-image-picker/Fable.ReactNativeImagePicker.fs.js";
import { singleton, ofArray } from "../.fable/fable-library.3.0.0/List.js";
import { now } from "../.fable/fable-library.3.0.0/Date.js";
import { Props_ViewProperties_Style_7FB9FF3D, Props_ViewStyle, Props_TextStyle, Props_TextInput_TextInputProperties_Style_7FB9FF3D, Props_TextInput_TextInputProperties, Props_FlexStyle, Props_ImageStyle, Props_ImageProperties_Style_7FB9FF3D, Props_ImageProperties, Toast_showShort } from "../.fable/Fable.React.Native.3.0.0-alpha001/Fable.ReactNative.fs.js";
import { defaultText, sceneBackground, button } from "../Styles.js";
import * as react from "react";
import { TextInput, Text as Text$, View, Image } from "react-native";
import { keyValueList } from "../.fable/fable-library.3.0.0/MapUtil.js";

export class Status extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Unchanged", "Changed", "Error"];
    }
}

export function Status$reflection() {
    return union_type("CheckLocation.Status", [], Status, () => [[], [], [["Item", string_type]]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["PictureSelected", "LocationStatusUpdated", "SelectPicture", "SaveAndGoBack", "GoBack", "Error"];
    }
}

export function Msg$reflection() {
    return union_type("CheckLocation.Msg", [], Msg, () => [[["Item", option_type(string_type)]], [["Item", LocationStatus$reflection()]], [], [], [], [["Item", class_type("System.Exception")]]]);
}

export class Model extends Record {
    constructor(Position, LocationCheckRequest, PictureUri, Status) {
        super();
        this.Position = (Position | 0);
        this.LocationCheckRequest = LocationCheckRequest;
        this.PictureUri = PictureUri;
        this.Status = Status;
    }
}

export function Model$reflection() {
    return record_type("CheckLocation.Model", [], Model, () => [["Position", int32_type], ["LocationCheckRequest", LocationCheckRequest$reflection()], ["PictureUri", option_type(string_type)], ["Status", Status$reflection()]]);
}

export function init(pos, request) {
    return [new Model(pos, request, void 0, new Status(0)), Cmd_none()];
}

export function save(pos, request) {
    const key = "models/Model.LocationCheckRequest";
    let pr_1;
    const pr = AsyncStorage.getItem(key);
    pr_1 = (pr.then(((_arg1) => {
        if (_arg1 === null) {
            return [];
        }
        else {
            const v = _arg1;
            return Auto_unsafeFromString_Z5CB6BD(v, void 0, void 0, {
                ResolveType: () => array_type(LocationCheckRequest$reflection()),
            });
        }
    })));
    return pr_1.then(((model) => {
        model[pos] = request;
        const newModel = Auto_toString_5A41365E(0, model, void 0, void 0, void 0, {
            ResolveType: () => array_type(LocationCheckRequest$reflection()),
        });
        return AsyncStorage.setItem(key, newModel);
    }));
}

export function selectImage() {
    return Helpers_showImagePickerAsync(ofArray([new Props_ImagePickerOptions(0, "Take picture"), new Props_ImagePickerOptions(12, true)]));
}

export function update(msg, model) {
    let inputRecord;
    switch (msg.tag) {
        case 4: {
            return [model, Cmd_none()];
        }
        case 0: {
            const selectedPicture = msg.fields[0];
            return [new Model(model.Position, model.LocationCheckRequest, selectedPicture, new Status(1)), Cmd_none()];
        }
        case 2: {
            return [model, Cmd_OfPromise_either(selectImage, void 0, (arg0_1) => (new Msg(0, arg0_1)), (arg0_2) => (new Msg(5, arg0_2)))];
        }
        case 1: {
            const newStatus = msg.fields[0];
            return [new Model(model.Position, (inputRecord = model.LocationCheckRequest, new LocationCheckRequest_1(inputRecord.LocationId, inputRecord.Name, inputRecord.Address, newStatus, now())), model.PictureUri, new Status(1)), Cmd_none()];
        }
        case 5: {
            const e = msg.fields[0];
            Toast_showShort(e.message);
            return [new Model(model.Position, model.LocationCheckRequest, model.PictureUri, new Status(2, e.message)), Cmd_none()];
        }
        default: {
            if (model.Status.tag === 0) {
                return [model, Cmd_OfFunc_result(new Msg(4))];
            }
            else {
                return [model, Cmd_OfPromise_either((tupledArg) => save(tupledArg[0], tupledArg[1]), [model.Position, model.LocationCheckRequest], () => (new Msg(4)), (arg0) => (new Msg(5, arg0)))];
            }
        }
    }
}

export function view(model, dispatch) {
    let props_4, props_6, matchValue_1, s_1, props_8;
    const selectImageButton = button("Take picture", () => {
        dispatch(new Msg(2));
    });
    let image;
    const matchValue = model.PictureUri;
    if (matchValue == null) {
        const props_2 = ofArray([new Props_ImageProperties(8, require("${entryDir}/../images/snow.jpg")), Props_ImageProperties_Style_7FB9FF3D(ofArray([new Props_ImageStyle(5, "#000000"), new Props_FlexStyle(14, 3), new Props_FlexStyle(2, "center")]))]);
        image = react.createElement(Image, keyValueList(props_2, 1));
    }
    else {
        const uri = matchValue;
        const props = ofArray([new Props_ImageProperties(8, {
            uri: uri,
        }), Props_ImageProperties_Style_7FB9FF3D(ofArray([new Props_ImageStyle(5, "#000000"), new Props_FlexStyle(14, 3)]))]);
        image = react.createElement(Image, keyValueList(props, 1));
    }
    const props_10 = singleton(sceneBackground());
    return react.createElement(View, keyValueList(props_10, 1), (props_4 = singleton(defaultText()), react.createElement(Text$, keyValueList(props_4, 1), model.LocationCheckRequest.Name)), (props_6 = ofArray([new Props_TextInput_TextInputProperties(1, false), Props_TextInput_TextInputProperties_Style_7FB9FF3D(ofArray([new Props_FlexStyle(30, 2), new Props_FlexStyle(24, 2), new Props_TextStyle(0, "#FFFFFF"), new Props_ViewStyle(1, "#251D1C")])), new Props_TextInput_TextInputProperties(11, (arg_1) => {
        dispatch(new Msg(1, new LocationStatus(1, arg_1)));
    }), new Props_TextInput_TextInputProperties(25, (matchValue_1 = model.LocationCheckRequest.Status, (matchValue_1 != null) ? ((matchValue_1.tag === 1) ? (s_1 = matchValue_1.fields[0], s_1) : "") : ""))]), react.createElement(TextInput, keyValueList(props_6, 1))), image, selectImageButton, (props_8 = singleton(Props_ViewProperties_Style_7FB9FF3D(ofArray([new Props_FlexStyle(21, "center"), new Props_FlexStyle(1, "center"), new Props_FlexStyle(14, 1), new Props_FlexStyle(16, "row")]))), react.createElement(View, keyValueList(props_8, 1), button("Cancel", () => {
        dispatch(new Msg(4));
    }), button("OK", () => {
        dispatch(new Msg(3));
    }))));
}

