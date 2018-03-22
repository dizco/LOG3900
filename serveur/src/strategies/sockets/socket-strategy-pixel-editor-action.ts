import { WebSocketDecorator } from "../../decorators/websocket-decorator";
import { ClientPixelEditorAction } from "../../models/sockets/client-editor-action";
import { SocketStrategyEditorAction } from "./socket-strategy-editor-action";
import { PixelEditorActionDecorator } from "../../decorators/pixel-editor-action-decorator";
import { EditorActionDecorator } from "../../decorators/editor-action-decorator";
import { ServerEditorAction } from "../../models/sockets/server-editor-action";

export class SocketStrategyPixelEditorAction extends SocketStrategyEditorAction {
    protected clientAction: ClientPixelEditorAction;

    public constructor(clientAction: ClientPixelEditorAction) {
        super(clientAction);
        this.clientAction = clientAction;
    }

    protected buildEditorActionDecorator(wsDecorator: WebSocketDecorator): EditorActionDecorator {
        return new PixelEditorActionDecorator(this.clientAction, wsDecorator.user);
    }

    protected saveVisualChanges(message: ServerEditorAction): void {
        console.log("SocketStrategyPixelEditorAction SaveVisualChanges not implemented");
    }
}