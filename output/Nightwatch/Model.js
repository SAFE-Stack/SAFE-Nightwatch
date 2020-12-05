import { Record, Union } from "./.fable/fable-library.3.0.0/Types.js";
import { record_type, class_type, option_type, union_type, string_type } from "./.fable/fable-library.3.0.0/Reflection.js";

export class LocationStatus extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Ok", "Alarm"];
    }
}

export function LocationStatus$reflection() {
    return union_type("Model.LocationStatus", [], LocationStatus, () => [[], [["Item", string_type]]]);
}

export class LocationCheckRequest extends Record {
    constructor(LocationId, Name, Address, Status, Date$) {
        super();
        this.LocationId = LocationId;
        this.Name = Name;
        this.Address = Address;
        this.Status = Status;
        this.Date = Date$;
    }
}

export function LocationCheckRequest$reflection() {
    return record_type("Model.LocationCheckRequest", [], LocationCheckRequest, () => [["LocationId", string_type], ["Name", string_type], ["Address", string_type], ["Status", option_type(LocationStatus$reflection())], ["Date", option_type(class_type("System.DateTime"))]]);
}

