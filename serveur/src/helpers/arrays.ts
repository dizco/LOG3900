/**
 * Provides a way to insert items at a specified index of an array
 * @example InsertAllAtIndex([a, d, e], 1, [b, c]) => [a, b, c, d, e]
 * @param {T[]} arr
 * @param {number} index
 * @param {T} newItems
 * @returns {T[]}
 */
export function InsertAllAtIndex<T>(arr: T[], index: number, ...newItems: T[]): T[] {
    if (index < 0 || index > arr.length) {
        throw new Error("Index argument out of array bounds");
    }

    //Source: https://stackoverflow.com/a/38181008/6316091
    return [
        // part of the array before the specified index
        ...arr.slice(0, index),
        // inserted items
        ...newItems,
        // part of the array after the specified index
        ...arr.slice(index)
    ];
}