import * as WebSocket from "ws";
import { ServerEditorAction } from "../models/sockets/server-editor-action";
import { ClientEditorAction } from "../models/sockets/client-editor-action";
import { UserModel } from "../models/User";
import { UserFactory } from "../factories/user-factory";

export class EditorActionDecorator {
    private clientAction: ClientEditorAction;
    private user: UserModel;

    public constructor(clientAction: ClientEditorAction, user: UserModel) {
        this.clientAction = clientAction;
        this.user = user;
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
                id: this.clientAction.drawing.id,
                name: "Mona Lisa", //TODO: Fetch the rest of the drawing info by the id
                owner: {
                    id: 132,
                    username: "fred",
                    url: "https://example.com/users/fred",
                    avatar_url: "https://example.com/users/fred/avatar.jpg",
                }
            },
            author: UserFactory.build(this.user),
            stroke: this.clientAction.stroke,
            timestamp: Date.now(),
        });
    }
}
