import * as WebSocket from "ws";
import { ServerEditorAction } from "../models/sockets/server-editor-action";
import { ClientEditorAction } from "../models/sockets/client-editor-action";
import { UserModel } from "../models/User";

export abstract class EditorActionDecorator {
    protected clientAction: ClientEditorAction;
    protected user: UserModel;

    public constructor(clientAction: ClientEditorAction, user: UserModel) {
        this.clientAction = clientAction;
        this.user = user;
    }

    public abstract decorate(ws: WebSocket): ServerEditorAction;
}
