import * as express from "express";
import * as compression from "compression"; // compresses requests
import * as session from "express-session";
import * as bodyParser from "body-parser";
import * as logger from "morgan";
import * as lusca from "lusca";
import * as dotenv from "dotenv";
import * as mongo from "connect-mongo";
import * as mongoose from "mongoose";
import * as mongoosePaginate from "mongoose-paginate";
import * as passport from "passport";
import * as expressValidator from "express-validator";
import * as bluebird from "bluebird";
import * as cors from "cors";
import { NextFunction } from "express";
import { Request } from "express";
import { Response } from "express";

const MongoStore = mongo(session);

// Load environment variables from .env file, where API keys and passwords are configured
dotenv.config({ path: ".env" });

// Controllers (route handlers)
import * as userController from "./controllers/user";
import * as pingController from "./controllers/ping";
import * as drawingsController from "./controllers/drawings";
import * as templatesController from "./controllers/templates";
// API keys and Passport configuration
import * as passportConfig from "./config/passport";


// Create Express server
const app = express();

// Connect to MongoDB
const mongoUrl = process.env.MONGOLAB_URI;
(<any>mongoose).Promise = bluebird;
mongoose.connect(mongoUrl, { useMongoClient: true }).then(
    () => { /** ready to use. The `mongoose.connect()` promise resolves to undefined. */
    },
).catch(err => {
    console.log("MongoDB connection error. Please make sure MongoDB is running. " + err);
    // process.exit();
});
(<any>mongoosePaginate).paginate.options = {
    limit: 20,
};

// Express configuration
app.set("port", process.env.PORT || 3000);
app.use(compression());
app.use(logger("dev"));
app.use(bodyParser.json({ limit: "5mb" }));
app.use(bodyParser.urlencoded({ limit: "5mb", extended: true }));
app.use(expressValidator());
const mongoStore = new MongoStore({
    url: mongoUrl,
    autoReconnect: true
});
const sessionParser = session({
    resave: true,
    saveUninitialized: true,
    secret: process.env.SESSION_SECRET,
    store: mongoStore,
});
app.use(sessionParser);
app.use(passport.initialize());
app.use(passport.session());
app.use(lusca.xframe("SAMEORIGIN"));
app.use(lusca.xssProtection(true));
app.use(cors({credentials: true, preflightContinue: true}));
app.use((req: Request, res: Response, next: NextFunction) => {
    res.header("Access-Control-Allow-Origin", req.get("origin"));
    next();
});
app.use((req: Request, res: Response, next: NextFunction) => {
    //Hijacked from cors, needed to be able to set the origin before ending the response
    const method = req.method && req.method.toUpperCase && req.method.toUpperCase();
    if (method === "OPTIONS") {
        res.statusCode = 204;
        res.setHeader("Content-Length", "0");
        res.end();
    }
    else {
        next();
    }
});

app.use((req: Request, res: Response, next: NextFunction) => {
    res.locals.user = req.user;
    next();
});

const noCache = (req: Request, res: Response, next: NextFunction) => {
    res.header("Cache-Control", "private, no-cache, no-store, must-revalidate");
    res.header("Expires", "-1");
    res.header("Pragma", "no-cache");
    return next();
};

/**
 * Primary app routes.
 */
app.post("/login", userController.postLogin);
app.post("/logout", passportConfig.isAuthenticated, userController.logout);
app.post("/register", userController.postRegister);
app.put("/account/password", passportConfig.isAuthenticated, userController.putUpdatePassword);
//app.post("/account/delete", passportConfig.isAuthenticated, userController.postDeleteAccount);

/**
 * Drawings
 */
app.get("/drawings", drawingsController.getDrawings);
app.post("/drawings", passportConfig.isAuthenticated, drawingsController.postDrawing);
app.get("/drawings/:id", [passportConfig.isAuthenticated, noCache], drawingsController.getDrawing);
app.get("/drawings/:id/thumbnail", drawingsController.getDrawingThumbnail);
app.get("/drawings/:id/actions", passportConfig.isAuthenticated, drawingsController.getDrawingActions);
app.patch("/drawings/:id", passportConfig.isAuthenticated, drawingsController.patchDrawing);
app.put("/drawings/:id/thumbnail", passportConfig.isAuthenticated, drawingsController.putDrawingThumbnail);

/**
 * Templates
 */
app.get("/templates", templatesController.getTemplates);
app.post("/templates", templatesController.postTemplate);
app.get("/templates/:id", templatesController.getTemplate);
app.get("/templates/:id/thumbnail", templatesController.getTemplateThumbnail);
app.post("/templates/:id/likes", passportConfig.isAuthenticated, templatesController.postTemplateLike);

/**
 * Ping
 */
app.get("/ping", pingController.getPing);

export { app, sessionParser, mongoStore };