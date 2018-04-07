import * as BluebirdPromise from "bluebird";

export class Command {
    private static MAX_TRIES = 2;
    private static TIMEOUT = 5000;
    private description: string;
    private tag: string;
    private tries: number;
    private command: () => BluebirdPromise<boolean>;

    public constructor(description: string, tag = "", command: () => BluebirdPromise<boolean>) {
        this.description = description;
        this.tries = 0;
        this.command = command;
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
