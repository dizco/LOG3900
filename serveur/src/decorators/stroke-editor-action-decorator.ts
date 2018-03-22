import * as WebSocket from "ws";
import { UserFactory } from "../factories/user-factory";
import { ServerStrokeEditorAction } from "../models/sockets/server-editor-action";
import { EditorActionDecorator } from "./editor-action-decorator";
import { UserModel } from "../models/User";
import { ClientStrokeEditorAction } from "../models/sockets/client-editor-action";

export class StrokeEditorActionDecorator extends EditorActionDecorator {
    protected clientAction: ClientStrokeEditorAction;

    public constructor(clientAction: ClientStrokeEditorAction, user: UserModel) {
        super(clientAction, user);
        this.clientAction = clientAction;
    }

    public decorate(ws: WebSocket): ServerStrokeEditorAction {
        return {
            type: "server.editor.stroke.action",
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