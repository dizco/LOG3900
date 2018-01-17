import { SocketMessage } from "../../models/sockets/socket-message";
import { SocketStrategy } from "../../models/sockets/socket-strategy";
import { ClientEditorAction } from "../../models/sockets/client-editor-action";
import { ClientChatMessage } from "../../models/sockets/client-chat-message";
import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { SocketStrategyEditorAction } from "./socket-strategy-editor-action";
import { SocketStrategyChatMessage } from "./socket-strategy-chat-message";

/**
 * Interpret the message and determine the best strategy to use
 */
export class SocketStrategyContext implements SocketStrategy {
    private strategy: SocketStrategy;

    public constructor(socketMessage: SocketMessage) {
        if (socketMessage.type === "client.chat.message") {
            this.strategy = new SocketStrategyChatMessage(<ClientChatMessage>socketMessage);
        }
        else if (socketMessage.type === "client.editor.action") {
            this.strategy = new SocketStrategyEditorAction(<ClientEditorAction>socketMessage);
        }
    }

    /**
     * Execute the selected strategy
     * @param {WebSocketDecorator} ws
     */
    public execute(ws: WebSocketDecorator): void {
        this.strategy.execute(ws);
    }
}