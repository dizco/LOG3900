import { Room } from "../../src/websockets/room";
import { WebSocketDecorator } from "../../src/decorators/websocket-decorator";
import { expect } from "chai";
import * as sinon from "sinon";
import { SinonSandbox } from "sinon";
import { FakeWebSocket } from "./fake-websocket";
import { WebSocketServer } from "../../src/websockets/websocket-server";
import * as http from "http";
import { DefaultRooms } from "../../src/websockets/default-rooms";

describe("websocket server", function() {
    describe("creation", function() {
        let httpServer: http.Server;

        beforeEach(function() {
            httpServer = http.createServer((req, res) => {
            });
        });

        it("should create a new websocket server", function() {
            const server = new WebSocketServer(httpServer);
            expect(server).to.not.be.undefined;
        });
    });

    describe("rooms management", function() {
        let sandbox: SinonSandbox;
        const roomId = "test";
        let server: WebSocketServer;
        beforeEach(function() {
            sandbox = sinon.sandbox.create();
            server = new WebSocketServer(http.createServer((req, res) => {
            }));
        });

        afterEach(function() {
            sandbox.restore();
        });

        it("should not find the room", function() {
            expect(server.findRoom(roomId)).to.be.undefined;
        });

        it("should create a new room if doesn't exist", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);
            server.joinRoom(roomId, user1);

            expect(server.findRoom(roomId)).to.exist;
            expect(server.findRoom(roomId)).to.be.instanceOf(Room);
            //expect(server.findRoom(roomId).getClients()).to.contain(user1);
        });

        it("should removeClient a client from a room and destroy the room", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);

            server.joinRoom(roomId, user1);
            expect(server.findRoom(roomId)).to.exist;
            server.removeClient(user1);
            expect(server.findRoom(roomId)).to.not.exist;
        });

        it("should removeClient a client from a room and keep the room", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const ws2 = new FakeWebSocket("ws://localhost");

            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);
            const user2 = new WebSocketDecorator(sandbox.spy() as any, ws2);

            server.joinRoom(roomId, user1);
            server.joinRoom(roomId, user2);
            expect(server.findRoom(roomId)).to.exist;

            server.removeClient(user1);
            expect(server.findRoom(roomId)).to.exist;
        });

        it("should not find a user if none has been added", function() {
            expect(server.userExists(sandbox.spy() as any)).to.be.false;
        });

        it("should find a user that has been added to general", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);
            const userModel = sandbox.spy() as any;
            user1.user = userModel;

            server.joinRoom(DefaultRooms.General, user1);

            expect(server.userExists(userModel)).to.be.true;
        });

        it("should not find a user that has not been added to general", function() {
            const ws1 = new FakeWebSocket("ws://localhost");
            const user1 = new WebSocketDecorator(sandbox.spy() as any, ws1);
            const userModel = sandbox.spy() as any;
            user1.user = userModel;

            server.joinRoom(DefaultRooms.Chat, user1);

            expect(server.userExists(userModel)).to.be.false;
        });
    });
});
