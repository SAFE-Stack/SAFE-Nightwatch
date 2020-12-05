import { view as view_1, update as update_1, init as init_1, Msg } from "./Scenes/App.js";
import { Helpers_setOnHardwareBackPressHandler } from "./.fable/Fable.React.Native.3.0.0-alpha001/Fable.ReactNative.fs.js";
import { Cmd_none, Cmd_map, Cmd_ofSub, Cmd_batch } from "./.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { ofArray, singleton } from "./.fable/fable-library.3.0.0/List.js";
import { Program_withReactNative } from "./.fable/Fable.Elmish.React.3.0.1/react-native.fs.js";
import { ProgramModule_map, ProgramModule_runWith, ProgramModule_mkProgram, ProgramModule_withSubscription, ProgramModule_withConsoleTrace } from "./.fable/Fable.Elmish.3.1.0/program.fs.js";
import { Internal_saveState, Model$1, Msg$1, Internal_tryRestoreState } from "./.fable/Fable.Elmish.HMR.4.1.0/hmr.fs.js";
import { value as value_1 } from "./.fable/fable-library.3.0.0/Option.js";
import { uncurry } from "./.fable/fable-library.3.0.0/Util.js";

export function setupBackHandler(dispatch) {
    const backHandler = () => {
        dispatch(new Msg(1));
        return true;
    };
    Helpers_setOnHardwareBackPressHandler(backHandler);
}

export function subscribe(model) {
    return Cmd_batch(singleton(Cmd_ofSub((dispatch) => {
        setupBackHandler(dispatch);
    })));
}

(function () {
    const program_3 = Program_withReactNative("Nightwatch", ProgramModule_withConsoleTrace(ProgramModule_withSubscription(subscribe, ProgramModule_mkProgram(init_1, update_1, view_1))));
    let hmrState = null;
    const hot = module.hot;
    if (!(hot == null)) {
        window.Elmish_HMR_Count = ((window.Elmish_HMR_Count == null) ? 0 : (window.Elmish_HMR_Count + 1));
        const value = hot.accept();
        void undefined;
        const matchValue = Internal_tryRestoreState(hot);
        if (matchValue == null) {
        }
        else {
            const previousState = value_1(matchValue);
            hmrState = previousState;
        }
    }
    const map = (tupledArg) => {
        const model_3 = tupledArg[0];
        const cmd = tupledArg[1];
        return [model_3, Cmd_map((arg0) => (new Msg$1(0, arg0)), cmd)];
    };
    const mapUpdate = (update, msg_1, model_4) => {
        let msg_2, userModel, patternInput, newModel, cmd_2;
        const patternInput_1 = map((msg_1.tag === 1) ? [new Model$1(0), Cmd_none()] : (msg_2 = msg_1.fields[0], (model_4.tag === 1) ? (userModel = model_4.fields[0], (patternInput = update(msg_2, userModel), (newModel = patternInput[0], (cmd_2 = patternInput[1], [new Model$1(1, newModel), cmd_2])))) : [model_4, Cmd_none()]));
        const newModel_1 = patternInput_1[0];
        const cmd_3 = patternInput_1[1];
        hmrState = newModel_1;
        return [newModel_1, cmd_3];
    };
    const createModel = (tupledArg_1) => {
        const model_5 = tupledArg_1[0];
        const cmd_4 = tupledArg_1[1];
        return [new Model$1(1, model_5), cmd_4];
    };
    const mapInit = (init) => {
        if (hmrState == null) {
            return (arg_2) => createModel(map(init(arg_2)));
        }
        else {
            return (_arg1) => [hmrState, Cmd_none()];
        }
    };
    const mapSetState = (setState, model_6, dispatch_1) => {
        if (model_6.tag === 1) {
            const userModel_1 = model_6.fields[0];
            setState(userModel_1, (arg_3) => dispatch_1(new Msg$1(0, arg_3)));
        }
    };
    let hmrSubscription;
    const handler = (dispatch_2) => {
        if (!(hot == null)) {
            hot.dispose((data) => {
                Internal_saveState(data, hmrState);
                return dispatch_2(new Msg$1(1));
            });
        }
    };
    hmrSubscription = singleton(handler);
    const mapSubscribe = (subscribe_1, model_7) => {
        if (model_7.tag === 1) {
            const userModel_2 = model_7.fields[0];
            return Cmd_batch(ofArray([Cmd_map((arg0_2) => (new Msg$1(0, arg0_2)), subscribe_1(userModel_2)), hmrSubscription]));
        }
        else {
            return Cmd_none();
        }
    };
    const mapView = (view, model_8, dispatch_3) => {
        if (model_8.tag === 1) {
            const userModel_3 = model_8.fields[0];
            return view(userModel_3, (arg_4) => dispatch_3(new Msg$1(0, arg_4)));
        }
        else {
            throw (new Error("\nYour are using HMR and this Elmish application has been marked as inactive.\n\nYou should not see this message\n                    "));
        }
    };
    ProgramModule_runWith(void 0, ProgramModule_map(uncurry(2, mapInit), mapUpdate, mapView, mapSetState, mapSubscribe, program_3));
})();

