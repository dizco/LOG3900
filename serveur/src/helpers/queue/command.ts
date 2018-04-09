import * as BluebirdPromise from "bluebird";
import { TaggedObject } from "./tagged-object";

export class Command implements TaggedObject {
    private static MAX_TRIES = 2;
    private tries: number;

    public constructor(private description: string, private tag: string,
                       private command: () => BluebirdPromise<boolean>) {
        this.tries = 0;
    }

    public getDescription(): string {
        return this.description;
    }

    public getTag(): string {
        return this.tag;
    }

    public canExecute(): boolean {
        return this.tries < Command.MAX_TRIES;
    }

    public execute(): BluebirdPromise<boolean> {
        this.tries++;
        return this.command();
    }

    public resetTries(): void {
        this.tries = 0;
    }
}
