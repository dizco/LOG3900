using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolyPaint.Models.PixelModels
{
    public class Tools
    {
        private Point _oldPosition;
        private Point _newPosition;

        private readonly WriteableBitmap _writeableBitmap;

        internal event EventHandler<List<Tuple<Point, string>>> DrewLineEvent;

        public Tools(WriteableBitmap writeableBitmap, Point oldPosition, Point newPosition)
        {
            _writeableBitmap = writeableBitmap;
            _oldPosition = oldPosition;
            _newPosition = newPosition;
        }

        /// <summary>
        ///     Draw a serie of pixel between a position and another
        ///     The DrawLine is used to gives a smoother rendering
        /// </summary>
        /// <param name="pixelSize"></param>
        /// <param name="selectedColor"></param>
        /// <param name="isColored"></param>
        public void DrawPixel(int pixelSize, string selectedColor)
        {
            _drawnPixels.Clear();
            Color color = (Color) ColorConverter.ConvertFromString(selectedColor);

            for (int j = 0; j < pixelSize; j++)
            {
                for (int i = 0; i < pixelSize; i++)
                {
                    int x1 = (int)_oldPosition.X - i;
                    int x2 = (int) _newPosition.X - i;
                    int y1 = (int)_oldPosition.Y - j;
                    int y2 = (int)_newPosition.Y - j;

                    _writeableBitmap.DrawLine(x1, y1, x2, y2, color);
                    GeneratePixels(x1, y1, x2, y2, color);
                }
            }

            OnDrewLine();
        }

        private readonly List<Tuple<Point, string>> _drawnPixels = new List<Tuple<Point, string>>();

        private void GeneratePixels(int x1, int y1, int x2, int y2, Color color)
        {
            const int doublePrecision = 4;

            int dx = x2 - x1;
            int dy = y2 - y1;

            int lengthX = dx >= 0 ? dx : -dx;
            int lengthY = dy >= 0 ? dy : -dy;

            int stepRatio = lengthX > lengthY ? lengthX : lengthY;

            double stepX = Math.Round((double)dx / stepRatio,doublePrecision);
            double stepY = Math.Round((double)dy / stepRatio,doublePrecision);

            int stepCount = stepRatio == lengthX ? (int)(dx / stepX) : (int)(dy / stepY);

            for (int i = 0; i < stepCount; i++)
            {
                Point pixel = new Point(x1 + i * stepX, y1 + i * stepY);
                _drawnPixels.Add(new Tuple<Point, string>(pixel, color.ToString()));
            }
        }

        private void OnDrewLine()
        {
            DrewLineEvent?.Invoke(this, _drawnPixels);
        }
    }
}
