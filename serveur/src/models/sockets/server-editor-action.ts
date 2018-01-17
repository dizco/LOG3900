import { SocketMessage } from "./socket-message";

export interface ServerEditorAction extends SocketMessage {
    action: {
        id: number | string;
        name: string;
    };

    drawing: {
        id: number | string;
        name: string;
        owner: {
            id: number | string;
            username: string;
            name: string;
            url: string;
            avatar_url: string;
        }
    };

    author: {
        id: number | string;
        username: string;
        name: string;
        url: string;
        avatar_url: string;
    };
}
