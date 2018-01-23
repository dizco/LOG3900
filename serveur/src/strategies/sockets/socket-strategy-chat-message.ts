import { SocketStrategy } from "../../models/sockets/socket-strategy";
import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { ClientChatMessage } from "../../models/sockets/client-chat-message";
import { ServerChatMessage } from "../../models/sockets/server-chat-message";
import { ChatMessageDecorator } from "../../decorators/chat-message-decorator";

export class SocketStrategyChatMessage implements SocketStrategy {
    private clientMessage: ClientChatMessage;

    public constructor(clientMessage: ClientChatMessage) {
        this.clientMessage = clientMessage;
    }

    /**
     * Decorate the message received by adding info, then broadcast to others in the chat
     * @param {WebSocketDecorator} wsDecorator
     */
    public execute(wsDecorator: WebSocketDecorator): void {
        const decorator = new ChatMessageDecorator(this.clientMessage);
        decorator.decorate(wsDecorator.getWs())
            .then((message: ServerChatMessage) => {
                wsDecorator.broadcast.send(JSON.stringify(message)); //to(message.room.id.toString())
            })
            .catch((reason => console.log("ChatMessage failed", reason)));
    }
}