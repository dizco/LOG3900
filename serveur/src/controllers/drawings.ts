import { NextFunction, Request, Response } from "express";
import { default as Drawing, DrawingModel } from "../models/drawings/drawing";
import { PaginateResult } from "mongoose";
import { DrawingAttributes } from "../models/drawings/drawing-attributes";

const enum DrawingFields {
    Name = "name",
    Protection = "protection",
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
        select: "name",
        populate: [
            { path: "owner", select: "username" },
            ],
        page: req.query.page,
    };

    Drawing.paginate({}, options, (err: any, drawings: PaginateResult<DrawingModel>) => {
        if (err) {
            return next(err);
        }
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
    req.checkBody(DrawingFields.Protection, "Protection must be a boolean").optional().isBoolean();

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const drawing = new Drawing({ name: req.body[DrawingFields.Name], owner: req.user });
    if (req.body[DrawingFields.Protection]) {
        drawing.protected = true;
    }

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

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate drawing id.", hints: errors });
    }

    const populateOptions = [
        { path: "owner", select: "username" },
        { path: "actions.author", select: "username" }, //TODO: Decide if we want to send editor actions here or not
    ];
    Drawing.findOne({_id: req.params.id}).populate(populateOptions).exec((err: any, drawing: DrawingModel) => {
        if (err) {
            return next(err);
        }
        if (!drawing) {
            return res.status(404).json({ status: "error", error: "Drawing not found." });
        }
        return res.json(drawing);
    });
};

/**
 * PUT /drawings/:id
 */
export let putDrawing = (req: Request, res: Response) => {
    res.status(500).json({
        message: "PUT not implemented yet",
    });
};

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
        return res.json(drawing.actions);
    });
};

/**
 * PATCH /drawings/:id
 */
export let patchDrawing = (req: Request, res: Response, next: NextFunction) => {
    req.checkParams("id", "Drawing id cannot be empty").notEmpty();
    req.checkParams("id", "Id must be of type ObjectId").matches(/^[a-f\d]{24}$/i); //Match ObjectId : https://stackoverflow.com/a/20988824/6316091
    req.checkBody(DrawingFields.Protection, "Protection must be a boolean").optional().isBoolean();

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

function buildUpdateFields(req: Request): any {
    const fields: any = {};
    if (req.body[DrawingFields.ProtectionActive] !== undefined) {
        const password = (req.body[DrawingFields.ProtectionPassword] !== undefined) ? req.body[DrawingFields.ProtectionPassword] : "";
        fields.protection = {
            active: req.body[DrawingFields.ProtectionActive],
            password: password,
        };
    }
    return fields;
}
