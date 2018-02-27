import * as WebSocket from "ws";
import { ClientChatMessage } from "../models/sockets/client-chat-message";
import { ServerChatMessage } from "../models/sockets/server-chat-message";
import { UserModel } from "../models/User";
import { UserFactory } from "../factories/user-factory";

export class ChatMessageDecorator {
    private clientMessage: ClientChatMessage;
    private user: UserModel;

    public constructor(clientMessage: ClientChatMessage, user: UserModel) {
        this.clientMessage = clientMessage;
        this.user = user;
    }

    public decorate(ws: WebSocket): Promise<ServerChatMessage> {
        return Promise.resolve({
            type: "server.chat.message",
            message: this.clientMessage.message,
            room: {
                id: this.clientMessage.room.id,
                name: "Main Chat",
            },
            author: UserFactory.build(this.user),
            timestamp: Date.now(),
        });
    }
}
