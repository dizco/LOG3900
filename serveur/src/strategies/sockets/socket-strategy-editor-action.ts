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
                wsDecorator.broadcast.to(message.drawing.id.toString()).send(JSON.stringify(message));
            })
            .catch((reason => console.log("EditorAction failed", reason)));
    }
}