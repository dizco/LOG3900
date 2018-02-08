import * as request from "supertest";
import { app } from "../src/app";
import { expect } from "chai";

//TODO: UNSKIP these tests once the routes are implemented

describe.skip("POST /drawings", () => {
    it("should return 200 OK", (done) => {
        request(app).post("/drawings")
            .expect(200, done);
    });
});

describe.skip("GET /drawings/:id", () => {
    it("should return 200 OK", (done) => {
        request(app).get("/drawings/1") //TODO: Use a valid ID
            .expect(200, done);
    });
});

describe.skip("PUT /drawings/:id", () => {
    it("should return 200 OK", (done) => {
        request(app).put("/drawings/1") //TODO: Use a valid ID
            .expect(200, done);
    });
});