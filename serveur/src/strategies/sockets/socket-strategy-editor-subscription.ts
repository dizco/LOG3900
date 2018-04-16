import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { SocketStrategy } from "../../models/sockets/socket-strategy";
import { ClientEditorSubscription } from "../../models/sockets/client-editor-subscription";
import { EditorSubscriptionAction } from "../../websockets/editor-subscription-actions";

export class SocketStrategyEditorSubscription implements SocketStrategy {
    private clientSubscription: ClientEditorSubscription;

    public constructor(clientSubscription: ClientEditorSubscription) {
        this.clientSubscription = clientSubscription;
    }

    /**
     * Join or leave a WebSocket room
     * @param {WebSocketDecorator} wsDecorator
     */
    public execute(wsDecorator: WebSocketDecorator): void {
        console.log("Execute SocketStrategyEditorSubscription");
        const method = this.selectSubscription(wsDecorator);
        method(this.clientSubscription.drawing.id.toString());
    }

    private selectSubscription(wsDecorator: WebSocketDecorator): Function {
        if (this.clientSubscription.action.id === EditorSubscriptionAction.Join) {
            return (roomId: string) => { //TODO: Evaluate if possible to force remove the user of all drawing rooms
                wsDecorator.join(roomId);
                wsDecorator.acceptPolling(roomId); //We are ready to get polled
            };
        }
        else if (this.clientSubscription.action.id === EditorSubscriptionAction.Leave) {
            return (roomId: string) => wsDecorator.leave(roomId);
        }
        return (roomId: string) => console.log(`Failed to join/leave room ${roomId}. Make sure the action id is correct.`);
    }
}