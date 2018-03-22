import { SocketMessage } from "../../models/sockets/socket-message";
import { SocketStrategy } from "../../models/sockets/socket-strategy";
import {
    ClientEditorAction, IsClientStrokeEditorAction, IsClientPixelEditorAction,
    ClientStrokeEditorAction, ClientPixelEditorAction
} from "../../models/sockets/client-editor-action";
import { ClientChatMessage, IsClientChatMessage } from "../../models/sockets/client-chat-message";
import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { SocketStrategyStrokeEditorAction } from "./socket-strategy-stroke-editor-action";
import { SocketStrategyChatMessage } from "./socket-strategy-chat-message";
import { IsClientEditorSubscription } from "../../models/sockets/client-editor-subscription";
import { SocketStrategyEditorSubscription } from "./socket-strategy-editor-subscription";
import { SocketStrategyPixelEditorAction } from "./socket-strategy-pixel-editor-action";

/**
 * Interpret the message and determine the best strategy to use
 */
export class SocketStrategyContext implements SocketStrategy {
    private strategy: SocketStrategy;

    /**
     * It is assumed that canParse has been validated before
     * @param {SocketMessage} socketMessage
     */
    public constructor(socketMessage: SocketMessage) {
        this.selectStrategy(socketMessage);
    }

    /**
     * Execute the selected strategy
     * @param {WebSocketDecorator} ws
     */
    public execute(ws: WebSocketDecorator): void {
        this.strategy.execute(ws);
    }

    /**
     * Tries to interpret a socket message, returns false if is not recognized
     * @param {SocketMessage} socketMessage
     * @returns {boolean}
     */
    public static canParse(socketMessage: SocketMessage): boolean {
        if (socketMessage.type === "client.chat.message") {
            return IsClientChatMessage(socketMessage);
        }
        else if (socketMessage.type === "client.editor.stroke.action") {
            return IsClientStrokeEditorAction(socketMessage);
        }
        else if (socketMessage.type === "client.editor.pixel.action") {
            return IsClientPixelEditorAction(socketMessage);
        }
        else if (socketMessage.type === "client.editor.subscription") {
            return IsClientEditorSubscription(socketMessage);
        }
        return false;
    }

    /**
     * Parse the socket message according to the type
     * @param {SocketMessage} socketMessage
     */
    private selectStrategy(socketMessage: SocketMessage): void {
        if (socketMessage.type === "client.chat.message") {
            this.strategy = new SocketStrategyChatMessage(<ClientChatMessage>socketMessage);
        }
        else if (socketMessage.type === "client.editor.stroke.action") {
            this.strategy = new SocketStrategyStrokeEditorAction(<ClientStrokeEditorAction>socketMessage);
        }
        else if (socketMessage.type === "client.editor.pixel.action") {
            this.strategy = new SocketStrategyPixelEditorAction(<ClientPixelEditorAction>socketMessage);
        }
        else if (socketMessage.type === "client.editor.subscription") {
            this.strategy = new SocketStrategyEditorSubscription(<ClientEditorAction>socketMessage);
        }
    }
}
