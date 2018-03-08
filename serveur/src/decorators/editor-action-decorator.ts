import * as WebSocket from "ws";
import { ServerEditorAction } from "../models/sockets/server-editor-action";
import { ClientEditorAction } from "../models/sockets/client-editor-action";
import { UserModel } from "../models/User";
import { UserFactory } from "../factories/user-factory";
import { default as Drawing, DrawingModel } from "../models/drawings/drawing";
import { DrawingAttributes } from "../models/drawings/drawing-attributes";

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
                const drawingObject: DrawingAttributes = <DrawingAttributes>drawing.toObject();
                return resolve({
                    type: "server.editor.action",
                    action: {
                        id: this.clientAction.action.id,
                        name: this.clientAction.action.name,
                    },
                    drawing: {
                        id: this.clientAction.drawing.id,
                        name: drawingObject.name,
                        protection: drawingObject.protection,
                        owner: UserFactory.build(<any>drawingObject.owner),
                    },
                    author: UserFactory.build(this.user),
                    delta: this.clientAction.delta,
                    timestamp: Date.now(),
                });
            });
        });
    }
}
