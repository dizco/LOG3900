import * as mongoose from "mongoose";
import { NextFunction, Request, Response } from "express";
import { default as Template, TemplateModel } from "../models/template";
import { PaginateResult } from "mongoose";
import { default as Drawing, DrawingModel } from "../models/drawings/drawing";
import { default as Like } from "../models/like";

const enum TemplateFields {
    DrawingId = "drawing-id",
}

/**
 * GET /templates
 */
export let getTemplates = (req: Request, res: Response, next: NextFunction) => {
    req.checkQuery("page", "Page number must be an integer above 0").isInt({ min: 1 });

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const options = {
        select: "name mode thumbnail",
        populate: [
            { path: "owner", select: "username" },
        ],
        page: req.query.page,
    };

    Template.paginate({}, options, (err: any, templates: PaginateResult<TemplateModel>) => {
        if (err) {
            return next(err);
        }

        return res.json(templates);
    }).catch((reason => {
        console.log("Failed to fetch and paginate templates", reason);
        return next(reason);
    }));
};

/**
 * POST /templates
 */
export let postTemplate = (req: Request, res: Response, next: NextFunction) => {
    req.checkBody(TemplateFields.DrawingId, "Template drawing id cannot be empty").notEmpty();
    req.checkBody(TemplateFields.DrawingId, "Drawing id must be of type ObjectId").matches(/^[a-f\d]{24}$/i);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Validation errors.", hints: errors });
    }

    const populateOptions = [
        { path: "owner" },
    ];
    Drawing.findOne({ _id: req.body[TemplateFields.DrawingId] }).populate(populateOptions).exec((err: any, drawing: DrawingModel) => {
        if (err) {
            return next(err);
        }
        if (!drawing) {
            return res.status(404).json({ status: "error", error: "Drawing not found." });
        }

        const template = new Template({
            name: drawing.name,
            mode: drawing.mode,
            owner: drawing.owner,
            strokes: drawing.strokes,
            pixels: drawing.pixels,
            thumbnail: drawing.thumbnail,
        });

        template.save((err) => {
            if (err) {
                return next(err);
            }
            return res.json({ status: "success", objectId: template.id });
        });
    });
};

/**
 * GET /templates/:id
 */
export let getTemplate = (req: Request, res: Response, next: NextFunction) => {
    req.checkParams("id", "Drawing id cannot be empty").notEmpty();
    req.checkParams("id", "Id must be of type ObjectId").matches(/^[a-f\d]{24}$/i);

    const errors = req.validationErrors();

    if (errors) {
        return res.status(422).json({ status: "error", error: "Failed to validate template id.", hints: errors });
    }

    const populateOptions = [
        { path: "owner", select: "username" },
    ];
    Template.findOne({_id: req.params.id}).populate(populateOptions).exec((err: any, template: TemplateModel) => {
        if (err) {
            return next(err);
        }
        if (!template) {
            return res.status(404).json({ status: "error", error: "Template not found." });
        }

        return res.json(template.toObject({ versionKey: false }));
    });
};

