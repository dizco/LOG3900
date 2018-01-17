import * as WebSocket from "ws";
import { ServerEditorAction } from "../models/sockets/server-editor-action";
import { ClientEditorAction } from "../models/sockets/client-editor-action";
import { SocketStrategy } from "../models/sockets/socket-strategy";

export class EditorActionDecorator {
    private clientAction: ClientEditorAction;

    public constructor(clientAction: ClientEditorAction) {
        this.clientAction = clientAction;
    }

    public decorate(ws: WebSocket): Promise<ServerEditorAction> {
        //TODO: Build actual user data

        return Promise.resolve({
            type: "server.editor.action",
            action: {
                id: this.clientAction.action.id,
                name: this.clientAction.action.name,
            },
            drawing: {
                id: this.clientAction.drawing.id, //TODO: Fetch the rest of the drawing info by the id
                name: "Mona Lisa",
                owner: {
                    id: 132,
                    username: "fred",
                    name: "Frédéric",
                    url: "https://example.com/users/fred",
                    avatar_url: "https://example.com/users/fred/avatar.jpg",
                }
            },
            author: {
                id: 134,
                username: "dizco",
                name: "Gabriel",
                url: "https://example.com/users/dizco",
                avatar_url: "https://example.com/users/dizco/avatar.jpg",
            },
        });
    }
}
