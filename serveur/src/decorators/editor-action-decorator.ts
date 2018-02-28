import * as WebSocket from "ws";
import { ServerEditorAction } from "../models/sockets/server-editor-action";
import { ClientEditorAction } from "../models/sockets/client-editor-action";
import { UserModel } from "../models/User";
import { UserFactory } from "../factories/user-factory";
import { default as Drawing, DrawingModel } from "../models/drawings/drawing";

export class EditorActionDecorator {
    private clientAction: ClientEditorAction;
    private user: UserModel;

    public constructor(clientAction: ClientEditorAction, user: UserModel) {
        this.clientAction = clientAction;
        this.user = user;
    }

    public decorate(ws: WebSocket): Promise<ServerEditorAction> {
        return new Promise<ServerEditorAction>((resolve: (value?: ServerEditorAction | PromiseLike<ServerEditorAction>) => void,
                                                reject: (reason?: any) => void) => {
            Drawing.findOne({_id: this.clientAction.drawing.id}).populate("owner").exec((err: any, drawing: DrawingModel) => {
                if (err) {
                    return reject(err);
                }
                if (!drawing) {
                    return reject("Drawing not found.");
                }
                return resolve({
                    type: "server.editor.action",
                    action: {
                        id: this.clientAction.action.id,
                        name: this.clientAction.action.name,
                    },
                    drawing: {
                        id: this.clientAction.drawing.id,
                        name: drawing.name,
                        owner: UserFactory.build(<any>drawing.owner),
                    },
                    author: UserFactory.build(this.user),
                    stroke: this.clientAction.stroke,
                    timestamp: Date.now(),
                });
            });
        });
    }
}
