import { SocketMessage } from "./socket-message";

export interface ClientChatMessage extends SocketMessage {
    message: string;

    room: {
        id: number | string;
    };
}

export function IsClientChatMessage(message: any): message is ClientChatMessage {
    message = <ClientChatMessage>message;
    if (!("message" in message)) {
        return false;
    }
    else if (!("room" in message)) {
        return false;
    }
    else if (!("id" in message.room)) {
        return false;
    }

    return true;
}
