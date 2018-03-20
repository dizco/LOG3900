import { expect } from "chai";
import { InsertAllAtIndex } from "../../src/helpers/arrays";

describe("arrays helper", function() {
    describe("InsertAllAtIndex", function() {
        it("should insert item in empty array", function() {
            const array: string[] = [];
            const result = InsertAllAtIndex(array, 0, "a");
            expect(result).to.deep.equal(["a"]);
        });

        it("should insert item after existing item", function() {
            const array: string[] = ["a"];
            const result = InsertAllAtIndex(array, 1, "b");
            expect(result).to.deep.equal(["a", "b"]);
        });

        it("should insert item before existing item", function() {
            const array: string[] = ["b"];
            const result = InsertAllAtIndex(array, 0, "a");
            expect(result).to.deep.equal(["a", "b"]);
        });

        it("should insert item between existing elements", function() {
            const array: string[] = ["a", "c", "d"];
            const result = InsertAllAtIndex(array, 1, "b");
            expect(result).to.deep.equal(["a", "b", "c", "d"]);
        });

        it("should insert many items between existing elements", function() {
            const array: string[] = ["a", "d"];
            const result = InsertAllAtIndex(array, 1, ...["b", "c"]);
            expect(result).to.deep.equal(["a", "b", "c", "d"]);
        });

        it("should throw if index over bounds", function() {
            const array: string[] = ["a", "b"];
            expect(InsertAllAtIndex.bind(InsertAllAtIndex, array, 3, ...["c", "d"])).to.throw();
        });

        it("should throw if index under bounds", function() {
            const array: string[] = ["c", "d"];
            expect(InsertAllAtIndex.bind(InsertAllAtIndex, array, -1, ...["a", "b"])).to.throw();
        });
    });
});
