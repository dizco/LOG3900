import { Observer } from "../observer/observer";
import { PriorityQueue } from "./priority-queue";
import { Command } from "./command";
import { default as chalk } from "chalk";

export class QueueObserver extends Observer {
    private queue: PriorityQueue<Command>;
    private running: boolean;

    constructor (queue: PriorityQueue<Command>) {
        super();
        this.running = false;

        //Never unregisters because the QueueObserver is expected to be static
        //Could potentially use something like this https://stackoverflow.com/questions/28885625/is-there-destructor-in-typescript
        //And then we could initialize the QueueObserver somewhere and also release it when necessary. The release would unregister
        queue.register(this);
        this.queue = queue;
    }

    public notify(): void {
        if (!this.running) { //Avoid having two commands running at once
            this.runQueue();
        }
    }

    private runQueue(): void {
        if (this.queue.isEmpty()) {
            this.running = false;
            return;
        }

        this.running = true;
        const command = this.queue.peek();
        if (command) {
            this.runCommand(command);
        }
        else {
            this.running = false;
        }
    }

    private runCommand(command: Command): void {
        if (!command.canExecute()) {
            this.delayFailedCommand(command);
            return this.runQueue();
        }
        command.execute().then((success: boolean) => {
            if (success) {
                if (process.env.NODE_ENV === "development") {
                    console.log(`${chalk.cyan("[QueueObserver]")} Command ran successfully. ${command.getDescription()}.`);
                }
                this.queue.dequeue(); //Remove the operation which was peeked
                //We *could* dequeue instead of peek, and then if the operation fails re-enqueue it, but we would lose the order
            }
            else {
                console.log(`${chalk.cyan("[QueueObserver]")} Command failed. ${command.getDescription()}.`);
            }
            return this.runQueue(); //Run next command or try again
        }).catch((reason: any) => {
            console.log(`${chalk.cyan("[QueueObserver]")} Command errored. ${command.getDescription()}. ${reason}`);
            return this.runQueue(); //Try again
        });
    }

    private delayFailedCommand(command: Command): void {
        this.queue.dequeue();
        //command.resetTries();
        //this.queue.enqueue(command);
        //TODO: Add command to a fails queue?
        console.log(chalk.bgRed(`${chalk.cyan("[QueueObserver]")} Ignoring failed command. ${command.getDescription()}. Queue size: ${this.queue.size()}`));
    }
}