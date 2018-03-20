export class Command {
    private static MAX_TRIES = 2;
    private tries: number;
    private command: () => Promise<boolean>;

    public constructor(command: () => Promise<boolean>) {
        this.tries = 0;
        this.command = command;
    }

    public canExecute(): boolean {
        return this.tries < Command.MAX_TRIES;
    }

    public execute(): Promise<boolean> {
        this.tries++;
        return this.command();
    }

    public resetTries(): void {
        this.tries = 0;
    }
}
