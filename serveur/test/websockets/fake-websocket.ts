import * as WebSocket from "ws";

export class FakeWebSocket extends WebSocket {
    public readyState = super.OPEN;

    public send({data, options, cb}: {data: any, options: { mask?: boolean; binary?: boolean; compress?: boolean; fin?: boolean }, cb?: (err: Error) => void}): void {
        //Do nothing
    }
}
