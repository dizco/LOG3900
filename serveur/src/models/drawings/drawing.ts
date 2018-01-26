import { Stroke } from "./stroke";
import { DrawingAttributes } from "./drawing-attributes";

export interface Drawing extends DrawingAttributes {
    strokes?: Stroke[];
}
