import { SocketMessage } from "./socket-message";
import { Author } from "../author";

export interface ServerChatMessage extends SocketMessage {
    message: string;

    room: {
        id: number | string;
        name: string;
    };

    author: Author;
}
