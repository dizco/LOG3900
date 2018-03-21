import { SocketMessage } from "./socket-message";
import { Author } from "./author";
import { Action } from "./action";
import { Delta } from "../drawings/delta";

export interface ServerEditorAction extends SocketMessage {
    action: Action;

    drawing: {
        id: number | string;
    };

    author: Author;

    delta: Delta;
}
