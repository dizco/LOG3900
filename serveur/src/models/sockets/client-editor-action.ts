import { SocketMessage } from "./socket-message";
import { Action, IsAction } from "./action";
import { Delta, IsDelta } from "../drawings/delta";

export interface ClientEditorAction extends SocketMessage {
    action: Action;

    drawing: {
        id: number | string;
    };

    delta: Delta;
}

export function IsClientEditorAction(action: any): action is ClientEditorAction {
    action = <ClientEditorAction>action;
    if (action === null || action === undefined) {
        return false;
    }
    else if (!("action" in action)) {
        return false;
    }
    else if (!(IsAction(action.action))) {
        return false;
    }
    else if (!("drawing" in action)) {
        return false;
    }
    else if (!("id" in action.drawing)) {
        return false;
    }
    else if (action.drawing.id === null || action.drawing.id === undefined) {
        return false;
    }
    else if (!("delta" in action)) {
        return false;
    }
    else if (!(IsDelta(action.delta))) {
        return false;
    }

    return true;
}
