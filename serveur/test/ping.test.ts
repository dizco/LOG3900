import * as request from "supertest";
import { app } from "../src/app";

describe("GET /ping", () => {
    it("should return 204 OK", (done) => {
        request(app).get("/ping")
            .expect(204, done);
    });
});
