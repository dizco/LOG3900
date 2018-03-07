import { NextFunction, Request, Response } from "express";
import { default as Drawing, DrawingModel } from "../models/drawings/drawing";
import { PaginateResult } from "mongoose";

const enum DrawingFields {
    Name = "name",
    ProtectionActive = "protection-active",
    ProtectionPassword = "protection-password",
}

/**
 * GET /drawings
 */
export let getDrawings = (req: Request, res: Response, next: NextFunction) => {
    req.checkQuery("page", "Page number must be an integer above 0").isInt({ min: 1 });

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const options = {
        select: "name protection",
        populate: [
            { path: "owner", select: "username" },
            ],
        page: req.query.page,
    };

    Drawing.paginate({}, options, (err: any, drawings: PaginateResult<DrawingModel>) => {
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
    validateProtectionParameters(req);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const drawing = new Drawing({
        name: req.body[DrawingFields.Name],
        owner: req.user,
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
    req.checkParams("id", "Drawing id cannot be empty").notEmpty();
    req.checkParams("id", "Id must be of type ObjectId").matches(/^[a-f\d]{24}$/i); //Match ObjectId : https://stackoverflow.com/a/20988824/6316091
    req.checkHeaders(DrawingFields.ProtectionPassword, "Protection password cannot be empty").optional().notEmpty();

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate drawing id.", hints: errors });
    }

    const populateOptions = [
        { path: "owner", select: "username" },
    ];
    Drawing.findOne({_id: req.params.id}, {actions: 0}).populate(populateOptions).exec((err: any, drawing: any) => {
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
 * GET /drawings/:id/actions
 */
export let getDrawingActions = (req: Request, res: Response, next: NextFunction) => {
    req.checkParams("id", "Drawing id cannot be empty").notEmpty();
    req.checkParams("id", "Id must be of type ObjectId").matches(/^[a-f\d]{24}$/i); //Match ObjectId : https://stackoverflow.com/a/20988824/6316091

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate drawing id.", hints: errors });
    }

    const populateOptions = [
        { path: "actions.author", select: "username" },
    ];
    Drawing.findOne({_id: req.params.id}).populate(populateOptions).exec((err: any, drawing: DrawingModel) => {
        if (err) {
            return next(err);
        }
        if (!drawing) {
            return res.status(404).json({ status: "error", error: "Drawing not found." });
        }
        return res.json((<any>drawing.toObject()).actions);
    });
};

/**
 * PATCH /drawings/:id
 */
export let patchDrawing = (req: Request, res: Response, next: NextFunction) => {
    req.checkParams("id", "Drawing id cannot be empty").notEmpty();
    req.checkParams("id", "Id must be of type ObjectId").matches(/^[a-f\d]{24}$/i); //Match ObjectId : https://stackoverflow.com/a/20988824/6316091
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
    return fields;
}

function validateProtectionParameters(req: Request): void {
    req.checkBody(DrawingFields.ProtectionActive, "Protection active must be a boolean").optional().isBoolean();
    if (req.body[DrawingFields.ProtectionActive]) {
        req.checkBody(DrawingFields.ProtectionPassword, "Protection password must be at least 4 characters long").len({ min: 4 });
    }
}
