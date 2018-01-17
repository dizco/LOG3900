import { ServerEditorAction } from "../models/sockets/server-editor-action";
import { ClientEditorAction } from "../models/sockets/client-editor-action";

export class EditorActionFactory {
    public static CreateServerEditorAction(socket: SocketIO.Socket, clientAction: ClientEditorAction): ServerEditorAction {
        //const clientId = socket.client.id;
        //TODO: Build actual user data

        return {
            action: {
                id: clientAction.action.id,
                name: clientAction.action.name,
            },
            drawing: {
                id: clientAction.drawing.id, //TODO: Fetch the rest of the drawing info by the id
                name: "Mona Lisa",
                owner: {
                    id: 132,
                    username: "fred",
                    name: "Frédéric",
                    url: "https://example.com/users/fred",
                    avatar_url: "https://example.com/users/fred/avatar.jpg",
                }
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
