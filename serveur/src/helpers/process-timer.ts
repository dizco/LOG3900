//Inspired by https://stackoverflow.com/a/14551263/6316091
export class ProcessTimer {
    private static readonly PRECISION = 3;
    private startTime: [number, number];
    private elapsedTime: number;

    public start(): void {
        this.startTime = process.hrtime();
    }

    public stop(): void {
        this.elapsedTime = process.hrtime(this.startTime)[1] / 1000000; // divide by a million to get nano to milli
    }

    /**
     * Returns a string with the elapsed time in ms
     * @param {number} precision
     * @returns {string} elapsed time in ms
     */
    public getElapsedTime(precision: number = ProcessTimer.PRECISION): string {
        return this.elapsedTime.toFixed(precision);
    }

    /**
     * Prints the time taken by the operation, only if in dev mode
     */
    public print(operation?: string): void {
        if (process.env.NODE_ENV === "development") {
            operation = (operation) ? "- " + operation : "";
            console.log(`Operations took ${this.getElapsedTime()} ms ${operation}`);
        }
    }
}