import * as request from "supertest";
import * as app from "../src/app";
import { expect } from "chai";

describe("GET /ping", () => {
    it("should return 200 OK", (done) => {
        request(app).get("/ping")
            .expect(200, done);
    });
});
