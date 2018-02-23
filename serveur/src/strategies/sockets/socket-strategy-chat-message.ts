import { SocketStrategy } from "../../models/sockets/socket-strategy";
import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { ClientChatMessage } from "../../models/sockets/client-chat-message";
import { ServerChatMessage } from "../../models/sockets/server-chat-message";
import { ChatMessageDecorator } from "../../decorators/chat-message-decorator";
import { PredefinedRooms } from "../../websockets/predefined-rooms";

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
        const decorator = new ChatMessageDecorator(this.clientMessage, wsDecorator.user);
        decorator.decorate(wsDecorator.getWs())
            .then((message: ServerChatMessage) => {
                //TODO: If we implement multiple chat rooms, message.room.id.toString()
                const success = wsDecorator.broadcast.to(PredefinedRooms.Chat).send(JSON.stringify(message));
                if (!success) {
                    console.log("ChatMessage failed to broadcast");
                    //TODO: Notify emitting user
                }
            })
            .catch((reason => console.log("ChatMessage failed to fetch db info", reason)));
    }
}