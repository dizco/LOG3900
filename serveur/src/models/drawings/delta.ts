import { Stroke } from "./stroke";

export interface Delta {
    add: Stroke[];
    remove: string[]; //Array of uuid
}

export function IsDelta(delta: any): delta is Delta {
    delta = <Delta>delta;
    if (delta === null || delta === undefined) {
        return false;
    }
    else if (!("add" in delta)) {
        return false;
    }
    else if (!("remove" in delta)) {
        return false;
    }

    return true;
}
