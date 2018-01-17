export interface ServerChatMessage {
    message: string;

    room: {
        id: number | string;
        name: string;
    };

    author: {
        id: number | string;
        username: string;
        name: string;
        url: string;
        avatar_url: string;
    };
}
