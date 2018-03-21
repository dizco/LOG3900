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

    public decorate(ws: WebSocket): ServerEditorAction {
        return {
            type: "server.editor.action",
            action: {
                id: this.clientAction.action.id,
                name: this.clientAction.action.name,
            },
            drawing: {
                id: this.clientAction.drawing.id,
            },
            author: UserFactory.build(this.user),
            delta: this.clientAction.delta,
            timestamp: Date.now(),
        };
    }
}
