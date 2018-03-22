import { SocketMessage } from "./socket-message";
import { Action, IsAction } from "./action";
import { Delta, IsDelta } from "../drawings/delta";
import { ColorPixel } from "../drawings/pixel";

export interface ClientEditorAction extends SocketMessage {
    action: Action;

    drawing: {
        id: number | string;
    };
}

export interface ClientStrokeEditorAction extends ClientEditorAction {
    delta: Delta;
}

export interface ClientPixelEditorAction extends ClientEditorAction {
    pixels: ColorPixel[];
}

function IsClientEditorAction(action: any): action is ClientEditorAction {
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

    return true;
}

export function IsClientStrokeEditorAction(action: any): action is ClientStrokeEditorAction {
    action = <ClientStrokeEditorAction>action;
    if (!IsClientEditorAction(action)) {
        return false;
    }
    else if (!("delta" in action)) {
        return false;
    }
    else if (!(IsDelta(action!.delta))) { //! avoids error :  Property 'delta' does not exist on type 'never'
        return false;
    }

    return true;
}

export function IsClientPixelEditorAction(action: any): action is ClientPixelEditorAction {
    action = <ClientPixelEditorAction>action;
    if (!IsClientEditorAction(action)) {
        return false;
    }
    else if (!("pixels" in action)) {
        return false;
    }

    return true;
}
