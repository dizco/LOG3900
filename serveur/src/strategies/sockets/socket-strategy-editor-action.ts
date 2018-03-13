import { SocketStrategy } from "../../models/sockets/socket-strategy";
import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { EditorActionDecorator } from "../../decorators/editor-action-decorator";
import { ClientEditorAction } from "../../models/sockets/client-editor-action";
import { ServerEditorAction } from "../../models/sockets/server-editor-action";
import { default as Drawing, DrawingModel } from "../../models/drawings/drawing";
import { default as User, UserModel } from "../../models/User";
import { ProcessTimer } from "../../helpers/process-timer";

export class SocketStrategyEditorAction implements SocketStrategy {
    private clientAction: ClientEditorAction;

    public constructor(clientAction: ClientEditorAction) {
        this.clientAction = clientAction;
    }

    /**
     * Decorate the message received by adding info, then broadcast to others in the room (drawing)
     * @param {WebSocketDecorator} wsDecorator
     */
    public execute(wsDecorator: WebSocketDecorator): void {
        const decorator = new EditorActionDecorator(this.clientAction, wsDecorator.user);
        decorator.decorate(wsDecorator.getWs())
            .then((message: ServerEditorAction) => {
                //TODO: Validate if user is allowed to broadcast to that room
                const success = wsDecorator.broadcast.to(message.drawing.id.toString()).send(JSON.stringify(message));
                if (success) {
                    SocketStrategyEditorAction.saveAction(message);
                }
                else {
                    console.log("EditorAction failed to broadcast");
                    //TODO: Notify emitting user
                }
            })
            .catch((reason => console.log("EditorAction failed to fetch db info", reason)));
    }

    private static saveAction(message: ServerEditorAction): void {
        const timer = new ProcessTimer();
        timer.start();
        User.findOne({ username: message.author.username }, (err: any, user: UserModel) => {
            if (err) {
                console.log("An error occurred while saving a user action", err);
            }
            else if (!user) {
                console.log("Could not find the author of the user action");
            }
            else {
                const action = { actionId: message.action.id, name: message.action.name, author: user, timestamp: message.timestamp };
                Drawing.findByIdAndUpdate(message.drawing.id, { $push: { actions: action }}, (err: any, drawing: DrawingModel) => {
                    if (err) {
                        console.log("An error occurred while saving a user action", err);
                    }
                    timer.stop();
                    timer.print("Save Editor Action");
                });
            }
        });
    }
}