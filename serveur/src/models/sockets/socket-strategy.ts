import { WebSocketDecorator } from "../../decorators/websocket-decorator";

export interface SocketStrategy {
    execute(wsDecorator: WebSocketDecorator): void;
}