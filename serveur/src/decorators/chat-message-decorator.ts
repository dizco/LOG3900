import * as WebSocket from "ws";
import { ClientChatMessage } from "../models/sockets/client-chat-message";
import { ServerChatMessage } from "../models/sockets/server-chat-message";

export class ChatMessageDecorator {
    private clientMessage: ClientChatMessage;

    public constructor(clientMessage: ClientChatMessage) {
        this.clientMessage = clientMessage;
    }

    public decorate(ws: WebSocket): Promise<ServerChatMessage> {
        //TODO: Build actual user data

        return Promise.resolve({
            type: "server.chat.message",
            message: this.clientMessage.message,
            room: {
                id: this.clientMessage.room.id,
                name: "Main Chat",
            },
            author: {
                id: 134,
                username: "dizco",
                name: "Gabriel",
                url: "https://example.com/users/dizco",
                avatar_url: "https://example.com/users/dizco/avatar.jpg",
            },
            timestamp: Date.now(),
        });
    }
}
