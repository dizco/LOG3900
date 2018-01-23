import { WebSocketServer } from "../../src/websockets/websocket-server";
import * as WebSocket from "ws";

export class FakeWebSocketServer extends WebSocketServer {
    public addClient(ws: WebSocket): void {
        this.clients.add(ws);
    }
}
