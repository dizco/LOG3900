import { Action, IsAction } from "./action";
import { SocketMessage } from "./socket-message";

export interface ClientEditorSubscription extends SocketMessage {
    action: Action;

    drawing: {
        id: number | string;
    };
}

export function IsClientEditorSubscription(subscription: any): subscription is ClientEditorSubscription {
    subscription = <ClientEditorSubscription>subscription;
    if (subscription === null || subscription === undefined) {
        return false;
    }
    else if (!("action" in subscription)) {
        return false;
    }
    else if (!(IsAction(subscription.action))) {
        return false;
    }
    else if (!("drawing" in subscription)) {
        return false;
    }
    else if (!("id" in subscription.drawing)) {
        return false;
    }
    else if (subscription.drawing.id === null || subscription.drawing.id === undefined) {
        return false;
    }

    return true;
}