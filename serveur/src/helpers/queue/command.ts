export class Command {
    private static MAX_TRIES = 2;
    private description: string;
    private tries: number;
    private command: () => Promise<boolean>;

    public constructor(description: string, command: () => Promise<boolean>) {
        this.description = description;
        this.tries = 0;
        this.command = command;
    }

    public getDescription(): string {
        return this.description;
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
