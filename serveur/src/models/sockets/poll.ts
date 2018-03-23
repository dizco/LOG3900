import { SocketMessage } from "./socket-message";

export interface Poll extends SocketMessage {
    drawing: {
        id: number | string;
    };
}