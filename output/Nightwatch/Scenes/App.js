import { Record, Union } from "../.fable/fable-library.3.0.0/Types.js";
import { record_type, list_type, union_type, int32_type } from "../.fable/fable-library.3.0.0/Reflection.js";
import { LocationCheckRequest$reflection } from "../Model.js";
import { view as view_3, update as update_3, init as init_2, Model$reflection as Model$reflection_1, Msg$reflection as Msg$reflection_1 } from "./Home.js";
import { view as view_2, update as update_1, init as init_3, Model$reflection as Model$reflection_2, Msg$reflection as Msg$reflection_2 } from "./LocationList.js";
import { view as view_1, update as update_2, init as init_1, Model$reflection as Model$reflection_3, Msg$reflection as Msg$reflection_3 } from "./CheckLocation.js";
import { Cmd_none, Cmd_OfFunc_result, Cmd_map } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { singleton, cons } from "../.fable/fable-library.3.0.0/List.js";
import { Helpers_exitApp } from "../.fable/Fable.React.Native.3.0.0-alpha001/Fable.ReactNative.fs.js";

export class Page extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Home", "LocationList", "CheckLocation"];
    }
}

export function Page$reflection() {
    return union_type("App.Page", [], Page, () => [[], [], [["Item1", int32_type], ["Item2", LocationCheckRequest$reflection()]]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["NavigateTo", "NavigateBack", "ExitApp", "HomeSceneMsg", "LocationListMsg", "CheckLocationMsg"];
    }
}

export function Msg$reflection() {
    return union_type("App.Msg", [], Msg, () => [[["Item", Page$reflection()]], [], [], [["Item", Msg$reflection_1()]], [["Item", Msg$reflection_2()]], [["Item", Msg$reflection_3()]]]);
}

export class SubModel extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["HomeModel", "LocationListModel", "CheckLocationModel"];
    }
}

export function SubModel$reflection() {
    return union_type("App.SubModel", [], SubModel, () => [[["Item", Model$reflection_1()]], [["Item", Model$reflection_2()]], [["Item", Model$reflection_3()]]]);
}

export class Model extends Record {
    constructor(SubModel, NavigationStack) {
        super();
        this.SubModel = SubModel;
        this.NavigationStack = NavigationStack;
    }
}

export function Model$reflection() {
    return record_type("App.Model", [], Model, () => [["SubModel", SubModel$reflection()], ["NavigationStack", list_type(Page$reflection())]]);
}

export function wrap(ctor, msgCtor, model, subModel, cmd) {
    return [new Model(ctor(subModel), model.NavigationStack), Cmd_map(msgCtor, cmd)];
}

export function navigateTo(page, newStack, model) {
    let tupledArg_3;
    switch (page.tag) {
        case 2: {
            const request = page.fields[1];
            const pos = page.fields[0] | 0;
            const tupledArg_1 = init_1(pos, request);
            tupledArg_3 = wrap((arg0_2) => (new SubModel(2, arg0_2)), (arg0_3) => (new Msg(5, arg0_3)), model, tupledArg_1[0], tupledArg_1[1]);
            break;
        }
        case 0: {
            const tupledArg_2 = init_2();
            tupledArg_3 = wrap((arg0_4) => (new SubModel(0, arg0_4)), (arg0_5) => (new Msg(3, arg0_5)), model, tupledArg_2[0], tupledArg_2[1]);
            break;
        }
        default: {
            const tupledArg = init_3();
            tupledArg_3 = wrap((arg0) => (new SubModel(1, arg0)), (arg0_1) => (new Msg(4, arg0_1)), model, tupledArg[0], tupledArg[1]);
        }
    }
    const model_1 = tupledArg_3[0];
    const cmd_3 = tupledArg_3[1];
    return [new Model(model_1.SubModel, newStack), cmd_3];
}

export function update(msg, model) {
    switch (msg.tag) {
        case 4: {
            const subMsg_1 = msg.fields[0];
            const matchValue_1 = model.SubModel;
            if (matchValue_1.tag === 1) {
                const subModel_2 = matchValue_1.fields[0];
                switch (subMsg_1.tag) {
                    case 1: {
                        return [model, Cmd_OfFunc_result(new Msg(1))];
                    }
                    case 0: {
                        const request = subMsg_1.fields[1];
                        const pos = subMsg_1.fields[0] | 0;
                        return [model, Cmd_OfFunc_result(new Msg(0, new Page(2, pos, request)))];
                    }
                    default: {
                        const tupledArg_1 = update_1(subMsg_1, subModel_2);
                        return wrap((arg0_2) => (new SubModel(1, arg0_2)), (arg0_3) => (new Msg(4, arg0_3)), model, tupledArg_1[0], tupledArg_1[1]);
                    }
                }
            }
            else {
                return [model, Cmd_none()];
            }
        }
        case 5: {
            const subMsg_2 = msg.fields[0];
            const matchValue_2 = model.SubModel;
            if (matchValue_2.tag === 2) {
                const subModel_4 = matchValue_2.fields[0];
                if (subMsg_2.tag === 4) {
                    return [model, Cmd_OfFunc_result(new Msg(1))];
                }
                else {
                    const tupledArg_2 = update_2(subMsg_2, subModel_4);
                    return wrap((arg0_4) => (new SubModel(2, arg0_4)), (arg0_5) => (new Msg(5, arg0_5)), model, tupledArg_2[0], tupledArg_2[1]);
                }
            }
            else {
                return [model, Cmd_none()];
            }
        }
        case 0: {
            const page = msg.fields[0];
            return navigateTo(page, cons(page, model.NavigationStack), model);
        }
        case 1: {
            const matchValue_3 = model.NavigationStack;
            let pattern_matching_result, page_1, rest;
            if (matchValue_3.tail != null) {
                if (matchValue_3.tail.tail != null) {
                    pattern_matching_result = 0;
                    page_1 = matchValue_3.tail.head;
                    rest = matchValue_3.tail.tail;
                }
                else {
                    pattern_matching_result = 1;
                }
            }
            else {
                pattern_matching_result = 1;
            }
            switch (pattern_matching_result) {
                case 0: {
                    return navigateTo(page_1, cons(page_1, rest), model);
                }
                case 1: {
                    return [model, Cmd_OfFunc_result(new Msg(2))];
                }
            }
        }
        case 2: {
            Helpers_exitApp();
            return [model, Cmd_none()];
        }
        default: {
            const subMsg = msg.fields[0];
            const matchValue = model.SubModel;
            if (matchValue.tag === 0) {
                const subModel = matchValue.fields[0];
                if (subMsg.tag === 2) {
                    return [model, Cmd_OfFunc_result(new Msg(0, new Page(1)))];
                }
                else {
                    const tupledArg = update_3(subMsg, subModel);
                    return wrap((arg0) => (new SubModel(0, arg0)), (arg0_1) => (new Msg(3, arg0_1)), model, tupledArg[0], tupledArg[1]);
                }
            }
            else {
                return [model, Cmd_none()];
            }
        }
    }
}

export function init() {
    const patternInput = init_2();
    const subModel = patternInput[0];
    const cmd = patternInput[1];
    return [new Model(new SubModel(0, subModel), singleton(new Page(0))), Cmd_map((arg0) => (new Msg(3, arg0)), cmd)];
}

export function view(model, dispatch) {
    const matchValue = model.SubModel;
    switch (matchValue.tag) {
        case 2: {
            const model_2 = matchValue.fields[0];
            return view_1(model_2, (arg_1) => {
                dispatch(new Msg(5, arg_1));
            });
        }
        case 1: {
            const model_3 = matchValue.fields[0];
            return view_2(model_3, (arg_2) => {
                dispatch(new Msg(4, arg_2));
            });
        }
        default: {
            const model_1 = matchValue.fields[0];
            return view_3(model_1, (arg) => {
                dispatch(new Msg(3, arg));
            });
        }
    }
}

