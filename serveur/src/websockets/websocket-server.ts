import * as WebSocket from "ws";
import { ServerOptions } from "ws";
import * as http from "http";
import * as https from "https";
import { WebSocketDecorator } from "../decorators/websocket-decorator";
import { Room } from "./room";
import { VerifyClientCallbackSync, VerifyClientCallbackAsync } from "ws";
import { UserModel } from "../models/User";
import { DefaultRooms } from "./default-rooms";

/*
Extends the default WebSocket.Server to add Rooms support
 */
export class WebSocketServer extends WebSocket.Server {
    private rooms: Room[];

    public constructor(server: http.Server | https.Server, verifyClient?: VerifyClientCallbackSync | VerifyClientCallbackAsync) {
        const options: ServerOptions = {
            server: server,
            verifyClient: verifyClient,
        };
        super(options);

        this.rooms = [];
    }

    public findRoom(id: string): Room {
        return this.rooms.find((room: Room) => {
            return room.getId() === id;
        });
    }

    public joinRoom(id: string, client: WebSocketDecorator): void {
        let room = this.findRoom(id);
        if (!room) {
            room = new Room(id);
            this.rooms.push(room);
        }

        room.addClient(client);
    }

    public leaveRoom(id: string, client: WebSocketDecorator): void {
        const room = this.findRoom(id);
        if (room) {
            room.removeClient(client);
        }
        this.cleanupEmptyRooms();
    }

    public removeClient(client: WebSocketDecorator): void {
        this.rooms.forEach((room: Room) => {
            room.removeClient(client);
        });
        this.cleanupEmptyRooms();
    }

    public userExists(user: UserModel): boolean {
        const room = this.findRoom(DefaultRooms.General);
        if (!room) {
            return false; //If the General room does not exist, no user is yet registered
        }
        const client = room.getClients().find((client: WebSocketDecorator) => {
            return client.user.id === user.id;
        });
        return client !== undefined;
    }

    private cleanupEmptyRooms(): void {
        this.rooms = this.rooms.filter((room: Room) => {
            const isEmpty = room.isEmpty();
            if (isEmpty)
                room.stopPolling();
            return !isEmpty;
        });
    }
}
