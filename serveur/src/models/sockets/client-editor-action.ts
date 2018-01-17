import { SocketMessage } from "./socket-message";

export interface ClientEditorAction extends SocketMessage {
    action: {
        id: number | string;
        name: string;
    };

    drawing: {
        id: number | string;
    };
}