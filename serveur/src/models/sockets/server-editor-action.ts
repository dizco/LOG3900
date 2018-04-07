import { SocketMessage } from "./socket-message";
import { Author } from "./author";
import { Action } from "./action";
import { Delta } from "../drawings/delta";
import { ColorPixel } from "../drawings/pixel";

export interface ServerEditorAction extends SocketMessage {
    action: Action;

    drawing: {
        id: string;
    };

    author: Author;
}

export interface ServerStrokeEditorAction extends ServerEditorAction {
    delta: Delta;
}

export interface ServerPixelEditorAction extends ServerEditorAction {
    pixels: ColorPixel[];
}
