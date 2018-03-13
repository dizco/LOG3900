import { SocketMessage } from "./socket-message";

export interface ClientChatMessage extends SocketMessage {
    message: string;

    room: {
        id: number | string;
    };
}

export function IsClientChatMessage(message: any): message is ClientChatMessage {
    message = <ClientChatMessage>message;
    if (message === null || message === undefined) {
        return false;
    }
    else if (!("message" in message)) {
        return false;
    }
    else if (!("room" in message)) {
        return false;
    }
    else if (!("id" in message.room)) {
        return false;
    }
    else if (message.room.id === null || message.room.id === undefined) {
        return false;
    }

    return true;
}
