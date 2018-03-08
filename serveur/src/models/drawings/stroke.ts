import { Pixel } from "./pixel";
import { StrokeAttributes } from "./stroke-attributes";

export interface Stroke {
    strokeUuid: string;
    strokeAttributes: StrokeAttributes;
    dots: Pixel[];
}

export function IsStroke(stroke: any): stroke is Stroke {
    stroke = <Stroke>stroke;
    if (stroke === null || stroke === undefined) {
        return false;
    }
    else if (!("strokeUuid" in stroke)) {
        return false;
    }
    else if (!("strokeAttributes" in stroke)) {
        return false;
    }
    else if (!("dots" in stroke)) {
        return false;
    }

    return true;
}
