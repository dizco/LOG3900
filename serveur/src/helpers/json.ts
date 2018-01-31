//Source : https://stackoverflow.com/a/20392392/6316091

/**
 * Attempts to parse a string as json, returns false if fails
 * @param {string} jsonString
 * @returns {object | boolean} parsed object if success, false otherwise
 * @constructor
 */
export function TryParseJSON(jsonString: string): object | boolean {
    try {
        const o = JSON.parse(jsonString);

        // Handle non-exception-throwing cases:
        // Neither JSON.parse(false) or JSON.parse(1234) throw errors, hence the type-checking,
        // but... JSON.parse(null) returns null, and typeof null === "object",
        // so we must check for that, too. Thankfully, null is falsey, so this suffices:
        if (o && typeof o === "object") {
            return o;
        }
    }
    catch (e) { }

    return false;
}
