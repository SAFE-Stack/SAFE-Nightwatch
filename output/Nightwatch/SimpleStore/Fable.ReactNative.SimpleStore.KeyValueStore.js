import async$002Dstorage from "@react-native-community/async-storage";

const asyncStorage = async$002Dstorage;

export function getAllKeys() {
    return new Promise(((success, fail) => {
        const value = asyncStorage.getAllKeys(((err, keys) => {
            if ((!(err == null)) ? (!(err.message == null)) : false) {
                fail(err);
            }
            else {
                success(keys);
            }
        }));
        void value;
    }));
}

