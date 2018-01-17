import { ClientChatMessage } from "../models/sockets/client-chat-message";
import { ServerChatMessage } from "../models/sockets/server-chat-message";

export class ChatMessageFactory {
    public static CreateServerChatMessage(socket: SocketIO.Socket, clientMessage: ClientChatMessage): ServerChatMessage {
        //const clientId = socket.client.id;
        //TODO: Build actual user data

        return {
            message: clientMessage.message,
            room: {
                id: 199,
                name: "Main Chat",
            },
            author: {
                id: 134,
                username: "dizco",
                name: "Gabriel",
                url: "https://example.com/users/dizco",
                avatar_url: "https://example.com/users/dizco/avatar.jpg",
            },
        };
    }
}
