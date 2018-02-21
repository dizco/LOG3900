import { SocketMessage } from "./socket-message";
import { Action, IsAction } from "./action";
import { IsStroke, Stroke } from "../drawings/stroke";

export interface ClientEditorAction extends SocketMessage {
    action: Action;

    drawing: {
        id: number | string;
    };

    stroke: Stroke;
}

export function IsClientEditorAction(action: any): action is ClientEditorAction {
    action = <ClientEditorAction>action;
    if (!("action" in action)) {
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
    else if (!("stroke" in action)) {
        return false;
    }
    else if (!(IsStroke(action.stroke))) {
        return false;
    }

    return true;
}
