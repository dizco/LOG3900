import { SocketStrategy } from "../../models/sockets/socket-strategy";
import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { EditorActionDecorator } from "../../decorators/editor-action-decorator";
import { ClientEditorAction } from "../../models/sockets/client-editor-action";
import { ServerEditorAction } from "../../models/sockets/server-editor-action";

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
        const decorator = new EditorActionDecorator(this.clientAction);
        decorator.decorate(wsDecorator.getWs())
            .then((message: ServerEditorAction) => {
                //TODO: Validate if user is allowed to broadcast to that room
                const success = wsDecorator.broadcast.to(message.drawing.id.toString()).send(JSON.stringify(message));
                if (!success) {
                    console.log("EditorAction failed to broadcast");
                    //TODO: Notify emitting user
                }
            })
            .catch((reason => console.log("EditorAction failed to fetch db info", reason)));
    }
}