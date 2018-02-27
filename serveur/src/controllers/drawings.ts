import { Request, Response } from "express";
import { Drawing } from "../models/drawings/drawing";
import { Stroke } from "../models/drawings/stroke";
import { Pixel } from "../models/drawings/pixel";
import { Owner } from "../models/drawings/owner";
import { ServerEditorAction } from "../models/sockets/server-editor-action";
import { StrokeAttributes } from "../models/drawings/stroke-attributes";
import { ClientEditorAction } from "../models/sockets/client-editor-action";
import { DrawingAttributes } from "../models/drawings/drawing-attributes";
import { UserFactory } from "../factories/user-factory";

/**
 * POST /drawings
 */
export let postDrawing = (req: Request, res: Response) => {
    //TODO: Assert that every mandatory field is filled out
    //TODO: Check cast to Drawing
    //TODO: Save new database entry with drawing properties and strokes
    //TODO: Send back json status
    res.status(500).json({
        message: "POST not implemented yet",
    });
};

/**
 * GET /drawings/:id
 */
export let getDrawing = (req: Request, res: Response) => {
    //TODO: Fetch drawing by id in database and cast to Drawing
    //TODO: Verify if drawing exists. If not, return error

    const owner: Owner = UserFactory.build(req.user);

    const drawingAttributes: DrawingAttributes = {
        id: req.params.id,
        name: "This is the way",
        owner: owner,
    };

    const dots: Pixel[] = [{x: 1, y: 2}];
    const strokeAttributes: StrokeAttributes = { color: "#FF000000", height: 10, width: 10, stylusTip: "Ellipse" };
    const actions: ServerEditorAction[] = [{
        type: "server.editor.action",
        action: { id: 1, name: "NewStroke" },
        drawing: drawingAttributes,
        author: UserFactory.build(req.user),
        stroke: { strokeAttributes: strokeAttributes, dots: dots},
    }];

    const drawing: Drawing = {
        id: drawingAttributes.id,
        name: drawingAttributes.name,
        owner: drawingAttributes.owner,
        actions: actions,
    };

    res.json(drawing);
};

/**
 * PUT /drawings/:id
 */
export let putDrawing = (req: Request, res: Response) => {
    res.status(500).json({
        message: "PUT not implemented yet",
    });
};