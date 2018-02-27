import { NextFunction, Request, Response } from "express";
import { default as Drawing, DrawingModel } from "../models/drawings/drawing";

const enum DrawingFields {
    Name = "name",
}

/**
 * POST /drawings
 */
export let postDrawing = (req: Request, res: Response, next: NextFunction) => {
    req.checkBody(DrawingFields.Name, "Drawing name cannot be empty").notEmpty();

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const drawing = new Drawing({ name: req.body[DrawingFields.Name], owner: req.user });

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
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    Drawing.findOne({_id: req.params.id}).populate("owner", "username").exec((err: any, drawing: DrawingModel) => {
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