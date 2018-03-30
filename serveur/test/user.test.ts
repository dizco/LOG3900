import * as request from "supertest";
import { app } from "../src/app";
import { expect } from "chai";

describe("POST /register", () => {
    it("should return some defined error message with valid parameters", (done) => {
        return request(app).post("/register")
            .field("email", "john@me.com")
            .field("password", "Hunter2")
            .expect(302)
            .end(function(err, res) {
                expect(res.error).not.to.be.undefined;
                done();
            });
    });
});

describe("POST /login", () => {
    it("should return some defined error message with valid parameters", (done) => {
        return request(app).post("/login")
            .field("email", "john@me.com")
            .field("password", "Hunter2")
            .expect(302)
            .end(function(err, res) {
                expect(res.error).not.to.be.undefined;
                done();
            });

    });
});
