import * as request from "supertest";
import { app } from "../src/app";

//TODO: Unskip this test if the route is implemented

describe.skip("GET /api", () => {
    it("should return 200 OK", () => {
        return request(app).get("/api")
            .expect(200);
    });
});
