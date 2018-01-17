import { SocketMessage } from "./socket-message";

export interface ClientChatMessage extends SocketMessage {
    message: string;
}
