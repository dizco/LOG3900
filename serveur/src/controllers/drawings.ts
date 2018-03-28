import { NextFunction, Request, Response } from "express";
import { default as Drawing, DrawingModel } from "../models/drawings/drawing";
import { PaginateResult } from "mongoose";
import Action, { ActionModel } from "../models/drawings/action";

const enum DrawingFields {
    Name = "name",
    Mode = "mode",
    Thumbnail = "thumbnail",
    Visibility = "visibility",
    ProtectionActive = "protection-active",
    ProtectionPassword = "protection-password",
}

/**
 * GET /drawings
 */
export let getDrawings = (req: Request, res: Response, next: NextFunction) => {
    validatePageParameters(req);
    req.checkParams("owner", "Owner must be of type ObjectId").optional().matches(/^[a-f\d]{24}$/i);
    validateVisibilityParameters(req);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const query: any = {};
    if (req.query.owner) { query.owner = req.query.owner; }
    if (req.query[DrawingFields.Visibility]) { query.visibility = req.query[DrawingFields.Visibility]; }
    const options = {
        select: "name mode protection visibility",
        populate: [
            { path: "owner", select: "username" },
            ],
        page: req.query.page,
    };

    Drawing.paginate(query, options, (err: any, drawings: PaginateResult<DrawingModel>) => {
        if (err) {
            return next(err);
        }
        drawings.docs = <any>drawings.docs.map(function(drawing) {
            return drawing.toObject(); //Transform every drawing
        });
        return res.json(drawings);
    }).catch((reason => {
        console.log("Failed to fetch and paginate drawings", reason);
        return next(reason);
    }));
};

/**
 * POST /drawings
 */
export let postDrawing = (req: Request, res: Response, next: NextFunction) => {
    req.checkBody(DrawingFields.Name, "Drawing name cannot be empty").notEmpty();
    validateModeParameters(req);
    validateVisibilityParameters(req);
    validateProtectionParameters(req);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const drawing = new Drawing({
        name: req.body[DrawingFields.Name],
        mode: req.body[DrawingFields.Mode],
        owner: req.user,
        visibility: req.body[DrawingFields.Visibility],
        protection: {
            active: protectionParameterIsActive(req),
            password: req.body[DrawingFields.ProtectionPassword],
        },
    });

    drawing.save((err) => {
        if (err) {
            return next(err);
        }
        return res.json({ status: "success", objectId: drawing.id });
    });
};

/**
 * GET /drawings/:id
 */
export let getDrawing = (req: Request, res: Response, next: NextFunction) => {
    validateIdParameters(req);
    req.checkHeaders(DrawingFields.ProtectionPassword, "Protection password cannot be empty").optional().notEmpty();

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate drawing id.", hints: errors });
    }

    const populateOptions = [
        { path: "owner", select: "username" },
    ];
    Drawing.findOne({_id: req.params.id}, {thumbnail: 0}).populate(populateOptions).exec((err: any, drawing: DrawingModel) => {
        if (err) {
            return next(err);
        }
        if (!drawing) {
            return res.status(404).json({ status: "error", error: "Drawing not found." });
        }

        if (drawing.protection.active) { //Password protected
            return handlePasswordProtectedDrawing(drawing, req, res, next);
        }
        else { //Not password protected
            return res.json(drawing.toObject({ versionKey: false }));
        }
    });
};

function handlePasswordProtectedDrawing(drawing: any, req: Request, res: Response, next: NextFunction) {
    drawing = <DrawingModel>drawing;
    if (drawing.owner.id === req.user.id) {
        console.log(`Bypassing drawing (id ${drawing.id}) password validation because user is owner.`);
        return res.json(drawing.toObject({ versionKey: false }));
    }
    if (req.headers[DrawingFields.ProtectionPassword] !== undefined) {
        drawing.comparePassword(req.headers[DrawingFields.ProtectionPassword], (err: Error, isMatch: boolean) => {
            if (err) {
                return next(err);
            }
            if (isMatch) {
                return res.json(drawing.toObject({ versionKey: false }));
            }
            return res.status(401).json({ status: "error", error: "Invalid password on password protected drawing." });
        });
    }
    else {
        return res.status(401).json({ status: "error", error: "Invalid password on password protected drawing." });
    }
}

/**
 * GET /drawings/:id/thumbnail
 */
export let getDrawingThumbnail = (req: Request, res: Response, next: NextFunction) => {
    validateIdParameters(req);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate drawing id.", hints: errors });
    }

    Drawing.findOne({_id: req.params.id}, {thumbnail: 1}, (err: any, drawing: DrawingModel) => {
        if (err) {
            return next(err);
        }
        if (!drawing) {
            return res.status(404).json({ status: "error", error: "Drawing not found." });
        }

        return res.json({ thumbnail: drawing.thumbnail });
    });
};

/**
 * GET /drawings/:id/actions
 */
export let getDrawingActions = (req: Request, res: Response, next: NextFunction) => {
    validateIdParameters(req);
    validatePageParameters(req);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate drawing id.", hints: errors });
    }

    const options = {
        select: "actionId name timestamp",
        sort: { timestamp: -1 }, //Newest come first
        populate: [
            { path: "author", select: "username" },
        ],
        page: req.query.page,
        limit: 40,
    };

    Action.paginate({ drawing: { _id: req.params.id }}, options, (err: any, actions: PaginateResult<ActionModel>) => {
        if (err) {
            return next(err);
        }
        actions.docs = <any>actions.docs.map(function(action) {
            return action.toObject(); //Keep actions
        });
        return res.json(actions);
    }).catch((reason => {
        console.log("Failed to fetch and paginate actions", reason);
        return next(reason);
    }));
};

/**
 * PATCH /drawings/:id
 */
export let patchDrawing = (req: Request, res: Response, next: NextFunction) => {
    validateIdParameters(req);
    validateVisibilityParameters(req);
    validateProtectionParameters(req);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate parameters or body.", hints: errors });
    }

    Drawing.findByIdAndUpdate(req.params.id, buildUpdateFields(req), (err: any, drawing: DrawingModel) => {
        if (err) {
            return next(err);
        }
        if (!drawing) {
            return res.status(404).json({ status: "error", error: "Drawing not found." });
        }
        return res.json({ status: "success" });
    });
};

/**
 * PUT /drawings/:id/thumbnail
 */
export let putDrawingThumbnail = (req: Request, res: Response, next: NextFunction) => {
    req.checkBody(DrawingFields.Thumbnail, "Drawing thumbnail must exist").exists();
    validateIdParameters(req);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate parameters or body.", hints: errors });
    }

    Drawing.findByIdAndUpdate(req.params.id, { thumbnail: req.body[DrawingFields.Thumbnail] }, (err: any, drawing: DrawingModel) => {
        if (err) {
            return next(err);
        }
        if (!drawing) {
            return res.status(404).json({ status: "error", error: "Drawing not found." });
        }
        return res.json({ status: "success" });
    });
};

function protectionParameterIsActive(req: Request): boolean {
    if (req.body[DrawingFields.ProtectionActive] !== undefined) {
        return req.body[DrawingFields.ProtectionActive].toLowerCase() === "true";
    }
    return false;
}

function buildUpdateFields(req: Request): any {
    const fields: any = {};
    if (req.body[DrawingFields.ProtectionActive] !== undefined) {
        const password = (req.body[DrawingFields.ProtectionPassword] !== undefined) ? req.body[DrawingFields.ProtectionPassword] : "";
        fields.protection = {
            active: protectionParameterIsActive(req),
            password: password,
        };
    }
    if (req.body[DrawingFields.Visibility] !== undefined) {
        fields.visibility = req.body[DrawingFields.Visibility];
    }
    return fields;
}

function validateIdParameters(req: Request): void {
    req.checkParams("id", "Drawing id cannot be empty").notEmpty();
    req.checkParams("id", "Id must be of type ObjectId").matches(/^[a-f\d]{24}$/i); //Match ObjectId : https://stackoverflow.com/a/20988824/6316091
}

function validatePageParameters(req: Request): void {
    req.checkQuery("page", "Page number must be an integer above 0").isInt({ min: 1 });
}

function validateModeParameters(req: Request): void {
    req.checkBody(DrawingFields.Mode, "Mode must be 'stroke' or 'pixel'").isIn(["stroke", "pixel"]);
}

function validateVisibilityParameters(req: Request): void {
    req.checkBody(DrawingFields.Visibility, "Visibility must be 'private' or 'public'").optional().isIn(["public", "private"]);
}

function validateProtectionParameters(req: Request): void {
    req.checkBody(DrawingFields.ProtectionActive, "Protection active must be a boolean").optional().isBoolean();
    if (protectionParameterIsActive(req)) {
        req.checkBody(DrawingFields.ProtectionPassword, "Protection password must be at least 5 characters long").len({ min: 5 });
    }
}
