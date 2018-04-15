﻿namespace PolyPaint.Models.PixelModels
{
    public static class GaussianBlur
    {
        public static int[,] KernelGaussianBlur7x7 = new int[7, 7]
        {
            {0, 0, 0, 5, 0, 0, 0},
            {0, 5, 18, 32, 18, 5, 0},
            {0, 18, 64, 100, 64, 18, 0},
            {5, 32, 100, 100, 100, 32, 5},
            {0, 18, 64, 100, 64, 18, 0},
            {0, 5, 18, 32, 18, 5, 0},
            {0, 0, 0, 5, 0, 0, 0}
        };

        public static int[,] KernelGaussianBlur9x9 = new int[9, 9]
        {
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 1, 6, 9, 6, 1, 0, 0},
            {0, 1, 15, 60, 95, 60, 15, 1, 0},
            {0, 6, 60, 240, 380, 240, 60, 6, 0},
            {0, 9, 95, 380, 600, 380, 95, 9, 0},
            {0, 6, 60, 240, 380, 240, 60, 6, 0},
            {0, 1, 15, 60, 95, 60, 15, 1, 0},
            {0, 0, 1, 6, 9, 6, 1, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0}
        };
    }
}
