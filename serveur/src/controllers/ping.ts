"use strict";

import { NextFunction, Request, Response } from "express";

/**
 * GET /ping
 * Ping API
 */
export let getPing = (req: Request, res: Response, next: NextFunction) => {
    res.status(204).send();
};
