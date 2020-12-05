import { PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "./.fable/Fable.Promise.2.1.0/Promise.fs.js";
import { promise } from "./.fable/Fable.Promise.2.1.0/PromiseImpl.fs.js";
import { Auto_toString_5A41365E } from "./.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { array_type, obj_type } from "./.fable/fable-library.3.0.0/Reflection.js";
import { AsyncStorage } from "./SimpleStore/Fable.ReactNative.SimpleStore.DB.js";
import { Auto_unsafeFromString_Z5CB6BD } from "./.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { LocationCheckRequest$reflection } from "./Model.js";
import { mapIndexed, append } from "./.fable/fable-library.3.0.0/Array.js";

export function createDemoData() {
    return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (PromiseBuilder__Delay_62FBFDE1(promise, () => {
        let key, s;
        return (key = "models/Model.LocationCheckRequest", (s = Auto_toString_5A41365E(0, [], void 0, void 0, void 0, {
            ResolveType: () => array_type(obj_type),
        }), AsyncStorage.setItem(key, s))).then((() => {
            let key_2, pr_1, pr;
            const requests = require("${entryDir}/../demodata/LocationCheckRequests.json");
            return (key_2 = "models/Model.LocationCheckRequest", (pr_1 = (pr = (AsyncStorage.getItem(key_2)), pr.then(((_arg1_1) => {
                if (_arg1_1 === null) {
                    return [];
                }
                else {
                    const v = _arg1_1;
                    return Auto_unsafeFromString_Z5CB6BD(v, void 0, void 0, {
                        ResolveType: () => array_type(LocationCheckRequest$reflection()),
                    });
                }
            }))), pr_1.then(((model) => {
                const newModel = Auto_toString_5A41365E(0, append(requests, model), void 0, void 0, void 0, {
                    ResolveType: () => array_type(LocationCheckRequest$reflection()),
                });
                return AsyncStorage.setItem(key_2, newModel);
            })))).then((() => (Promise.resolve(requests.length))));
        }));
    }).catch(((_arg3) => (Promise.resolve(0)))))));
}

export function getIndexedCheckRequests() {
    let pr_1;
    const key = "models/Model.LocationCheckRequest";
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
    return pr_1.then(((array) => mapIndexed((i, r) => [i, r], array)));
}

