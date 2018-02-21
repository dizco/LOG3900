import { DrawingAttributes } from "./drawing-attributes";
import { ServerEditorAction } from "../sockets/server-editor-action";

export interface Drawing extends DrawingAttributes {
    actions?: ServerEditorAction[];
}
