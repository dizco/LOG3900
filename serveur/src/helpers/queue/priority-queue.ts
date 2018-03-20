import { ObserverSubject } from "../observer/observer-subject";
import { Queue } from "./queue";
import { StandardQueue } from "./standard-queue";

export const enum QueueElementPriority {
    High = 0,
    Normal = 1,
    Low = 2,
}

//See https://basarat.gitbooks.io/algorithms/content/docs/datastructures/queue.html
//See http://code.iamkate.com/javascript/queues/
export class PriorityQueue<T> extends ObserverSubject implements Queue<T> {
    private queues: Queue<T>[] = [];

    public constructor() {
        super();
        this.queues.push(new StandardQueue()); //High
        this.queues.push(new StandardQueue()); //Normal
        this.queues.push(new StandardQueue()); //Low
    }

    public enqueue(val: T, priority: QueueElementPriority = QueueElementPriority.Normal): void {
        this.queues[priority].enqueue(val);
        this.notify(); //Tell observers that there is a new command to be run
    }

    /**
     * AVOID AT ALL COSTS
     * This method is completely non-conventional as it will insert the element in the middle of the queue
     * @param {T} val
     */
    /*public cowboyEnqueue(val: T) {
        this.queue = InsertAllAtIndex(this.queue, Math.floor(this.size() / 4), val);
        this.notify();
    }*/

    public dequeue(): T | undefined {
        const queue = this.queues
            .find(queue => !queue.isEmpty());
        return queue ? queue.dequeue() : undefined;
    }

    public isEmpty(): boolean {
        return this.queues
            .filter(queue => !queue.isEmpty())
            .length === 0;
    }

    public peek(): T | undefined {
        const queue = this.queues
            .find(queue => !queue.isEmpty());
        return queue ? queue.peek() : undefined;
    }

    public size(): number {
        let size = 0;
        this.queues.forEach(queue => size += queue.size());
        return size;
    }

    public clear(): void {
        this.queues.forEach(queue => queue.clear());
    }
}