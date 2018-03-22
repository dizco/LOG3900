import * as WebSocket from "ws";
import { UserFactory } from "../factories/user-factory";
import { ServerPixelEditorAction } from "../models/sockets/server-editor-action";
import { EditorActionDecorator } from "./editor-action-decorator";
import { UserModel } from "../models/User";
import { ClientPixelEditorAction } from "../models/sockets/client-editor-action";

export class PixelEditorActionDecorator extends EditorActionDecorator {
    protected clientAction: ClientPixelEditorAction;

    public constructor(clientAction: ClientPixelEditorAction, user: UserModel) {
        super(clientAction, user);
        this.clientAction = clientAction;
    }

    public decorate(ws: WebSocket): ServerPixelEditorAction {
        return {
            type: "server.editor.pixel.action",
            action: {
                id: this.clientAction.action.id,
                name: this.clientAction.action.name,
            },
            drawing: {
                id: this.clientAction.drawing.id,
            },
            author: UserFactory.build(this.user),
            pixels: this.clientAction.pixels,
            timestamp: Date.now(),
        };
    }
}