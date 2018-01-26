import { SocketMessage } from "./socket-message";
import { Action } from "./action";

export interface ClientEditorAction extends SocketMessage {
    action: Action;

    drawing: {
        id: number | string;
    };
}