import { SocketMessage } from "./socket-message";
import { Author } from "./author";
import { DrawingAttributes } from "../drawings/drawing-attributes";
import { Action } from "./action";

export interface ServerEditorAction extends SocketMessage {
    action: Action;

    drawing: DrawingAttributes; //We don't want to send the whole list of strokes every time

    author: Author;
}
