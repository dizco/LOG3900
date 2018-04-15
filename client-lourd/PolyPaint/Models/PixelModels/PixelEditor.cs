using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolyPaint.Models.PixelModels
{
    internal class PixelEditor : INotifyPropertyChanged
    {
        private const int ClockwiseAngle = 90; // degrees
        private const int CounterClockwiseAngle = 270; // degrees
        private const int AlphaShift = 24;
        private const int RedShift = 16;
        private const int GreenShift = 8;

        // Bitmap containing current selection
        private WriteableBitmap _cropWriteableBitmap;

        private Point _cropWriteableBitmapPosition;

        private bool _isWriteableBitmapOnEdition;

        private int _pixelSize = 5;

        private Tuple<Rect, Rect> _savedBlitSizes;

        private string _selectedColor = "Black";

        private int _blurIntensity = 1;

        private int _blurRadius = 1;

        // Active tool of the editor
        private string _selectedTool = "pencil";

        // Buffer containing what is behind current selection
        private WriteableBitmap _tempWriteableBitmap;

        // Original drawing
        private WriteableBitmap _writeableBitmap;

        public PixelEditor()
        {
            //Todo: Resize dynamically with the size of the Canvas
            WriteableBitmap = BitmapFactory.New(1000, 1000);
            WriteableBitmap.Clear(Colors.White);

            _cropWriteableBitmapPosition = new Point(0, 0);
        }

        public WriteableBitmap WriteableBitmap
        {
            get => _writeableBitmap;
            set
            {
                _writeableBitmap = value;
                PropertyModified();
            }
        }

        public WriteableBitmap CropWriteableBitmap
        {
            get => _cropWriteableBitmap;
            set
            {
                _cropWriteableBitmap = value;
                PropertyModified();
            }
        }

        public WriteableBitmap TempWriteableBitmap
        {
            get => _tempWriteableBitmap;
            set
            {
                _tempWriteableBitmap = value;
                PropertyModified();
            }
        }

        public Point CropWriteableBitmapPosition
        {
            get => _cropWriteableBitmapPosition;
            set
            {
                _cropWriteableBitmapPosition = value;
                PropertyModified();
            }
        }

        public bool IsWriteableBitmapOnEdition
        {
            get => _isWriteableBitmapOnEdition;
            set
            {
                _isWriteableBitmapOnEdition = value;
                PropertyModified();
            }
        }

        public string CurrentUsername { get; set; }

        public string DrawingName { get; set; }

        public string SelectedTool
        {
            get => _selectedTool;
            set
            {
                _selectedTool = value;
                PropertyModified();
            }
        }

        public string SelectedColor
        {
            get => _selectedColor;

            //When we select a color, it is generaly to draw after that
            //That's why when the color is changed, the tool is automaticaly changed for the pencil
            set
            {
                _selectedColor = value;
                SelectedTool = "pencil";
                PropertyModified();
            }
        }

        public int PixelSize
        {
            get => _pixelSize;
            set
            {
                _pixelSize = value;
                PropertyModified();
            }
        }

        public int BlurIntensity
        {
            get => _blurIntensity;
            set
            {
                _blurIntensity = value;
                PropertyModified();
            }
        }

        public int BlurRadius
        {
            get => _blurRadius;
            set
            {
                _blurRadius = value;
                PropertyModified();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler SelectedToolChanged;

        public event EventHandler<List<Tuple<Point, string>>> DrewPixelsEvent;

        /// <summary>
        ///     Event is raised when a selected area is blitted onto the drawing area
        /// </summary>
        public event EventHandler<Rect> ModifiedRegionEvent;

        public void ChangeCropWriteableBitmapPosition(Point position)
        {
            CropWriteableBitmapPosition = position;
        }

        /// <summary>
        ///     Draw on the writeableBitmap
        ///     Transparant color is put on the bitmap when the eraser is selected
        /// </summary>
        /// <param name="oldPosition"> Position of the mouse before an action </param>
        /// <param name="newPosition"> Position of the mouse after the actions </param>
        public void DrawPixels(Point oldPosition, Point newPosition)
        {
            Tools tools = new Tools(WriteableBitmap, oldPosition, newPosition);
            tools.DrewLineEvent += OnDrewPixels;
            if (SelectedTool == "pencil")
            {
                tools.DrawPixel(PixelSize, SelectedColor);
            }
            else if (SelectedTool == "eraser")
            {
                tools.DrawPixel(PixelSize, "White");
            }
        }

        public void DrawIncomingPixel(int x, int y, string pixelColor)
        {
            WriteableBitmap.SetPixel(x, y, (Color) ColorConverter.ConvertFromString(pixelColor));
        }

        /// <summary>
        ///     Select a zone in the drawing
        /// </summary>
        /// <param name="selectedZoneControl">Element controlling our selection</param>
        /// <param name="beginPosition">Point where the selection start</param>
        /// <param name="endPosition">Point where the selection end</param>
        public void SelectZone(ContentControl selectedZoneControl, Point beginPosition, Point endPosition)
        {
            Tools tools = new Tools(WriteableBitmap, beginPosition, endPosition);

            //The first point returns the lowest coordinates
            //The second point returns the highest coordinates
            Tuple<Point, Point> selectedRectangle = tools.SelectCropZone();

            //We crop a second WriteableBitmap and temporary Bitmap that will be edited into the size
            // of our selectedZone
            Rect rect = new Rect(selectedRectangle.Item1, selectedRectangle.Item2);
            CropWriteableBitmap = WriteableBitmap.Crop(rect);
            TempWriteableBitmap = WriteableBitmap.Crop(rect);
            TempWriteableBitmap.Clear(Colors.White);

            selectedZoneControl.Height = rect.Height;
            selectedZoneControl.Width = rect.Width;

            //We move our croppedwriteableBitmap into the position
            //of the selectedZone
            CropWriteableBitmapPosition = selectedRectangle.Item1;
            Canvas.SetLeft(selectedZoneControl, CropWriteableBitmapPosition.X);
            Canvas.SetTop(selectedZoneControl, CropWriteableBitmapPosition.Y);

            //Similar to MsPaint, the zoneSelected of our original writeableBitmap is filled with no color
            WriteableBitmap.FillRectangle((int) selectedRectangle.Item1.X, (int) selectedRectangle.Item1.Y,
                                          (int) selectedRectangle.Item2.X, (int) selectedRectangle.Item2.Y,
                                          Colors.White);

            //We can then start our edition
            IsWriteableBitmapOnEdition = true;
        }

        public void SelectTempZone(Point beginPosition, Point endPosition)
        {
            Tools tools = new Tools(WriteableBitmap, beginPosition, endPosition);

            //The first point returns the lower-left coordinates
            //The second point returns the uppe-right coordinates
            Tuple<Point, Point> selectedRectangle = tools.SelectCropZone();

            //We crop the WriteableBitmap destinaton
            Rect rect = new Rect(selectedRectangle.Item1, selectedRectangle.Item2);

            TempWriteableBitmap = WriteableBitmap.Crop(rect);
        }

        /// <summary>
        ///     Put the changes made by the user on the edited bitmap
        ///     Merge the edited bitmap on the original bitmap (the drawing)
        /// </summary>
        /// <param name="selectionContentControl">Control of the zone Selected</param>
        /// <param name="writeableBitmapSource">Writeablebitmap source that is blit on the original drawing</param>
        /// <param name="isSelectionOver">the selection ends if the value is set to True</param>
        public void BlitOnDrawing(ContentControl selectionContentControl, WriteableBitmap writeableBitmapSource,
            bool isSelectionOver, [Optional] Rect regionBeforeResize)
        {
            if (IsWriteableBitmapOnEdition)
            {
                writeableBitmapSource = writeableBitmapSource.Resize((int) selectionContentControl.Width,
                                                                     (int) selectionContentControl.Height,
                                                                     WriteableBitmapExtensions
                                                                         .Interpolation.NearestNeighbor);

                Point position = new Point(Canvas.GetLeft(selectionContentControl),
                                           Canvas.GetTop(selectionContentControl));

                Rect destinationRectangle = new Rect(position.X, position.Y,
                                                     writeableBitmapSource.PixelWidth,
                                                     writeableBitmapSource.PixelHeight);
                Rect sourceRectangle =
                    new Rect(0, 0, writeableBitmapSource.PixelWidth, writeableBitmapSource.PixelHeight);

                _savedBlitSizes = new Tuple<Rect, Rect>(destinationRectangle, sourceRectangle);

                WriteableBitmap.Blit(destinationRectangle, writeableBitmapSource, sourceRectangle);

                if (!isSelectionOver)
                {
                    if (!regionBeforeResize.IsEmpty)
                    {
                        double top = regionBeforeResize.Top < position.Y ? regionBeforeResize.Top : position.Y;
                        double left = regionBeforeResize.Left < position.X ? regionBeforeResize.Left : position.X;

                        double bottom = regionBeforeResize.Bottom > position.Y + writeableBitmapSource.PixelHeight
                                            ? regionBeforeResize.Bottom
                                            : position.Y + writeableBitmapSource.PixelHeight;
                        double right = regionBeforeResize.Right > position.X + writeableBitmapSource.PixelWidth
                                           ? regionBeforeResize.Right
                                           : position.X + writeableBitmapSource.PixelWidth;
                        destinationRectangle =
                            new Rect(new Point((int) left, (int) top), new Point((int) right, (int) bottom));
                    }

                    OnModifiedRegion(destinationRectangle);
                }
                else
                {
                    writeableBitmapSource.Clear(Colors.Transparent);
                    TempWriteableBitmap.Clear(Colors.Transparent);

                    IsWriteableBitmapOnEdition = false;

                    _savedBlitSizes = null;
                    selectionContentControl.Width = 0;
                    selectionContentControl.Height = 0;
                }
            }
        }

        public void ReloadTempWriteableBitmap()
        {
            if (_savedBlitSizes != null)
            {
                WriteableBitmap.Blit(_savedBlitSizes.Item1, TempWriteableBitmap, _savedBlitSizes.Item2);
            }
        }

        internal void QuarterTurnClockwise(ContentControl contentControl)
        {
            FillRegionBeforeRotation();
            QuarterTurnObject(contentControl);
            CropWriteableBitmap = CropWriteableBitmap.Rotate(ClockwiseAngle);
            UpdateModifiedRegion();
        }

        internal void QuarterTurnCounterClockwise(ContentControl contentControl)
        {
            FillRegionBeforeRotation();
            QuarterTurnObject(contentControl);
            CropWriteableBitmap = CropWriteableBitmap.Rotate(CounterClockwiseAngle);
            UpdateModifiedRegion();
        }

        internal void QuarterTurnObject(ContentControl contentControl)
        {
            Point relativePoint = new Point(Canvas.GetLeft(contentControl), Canvas.GetTop(contentControl));
            Point middlePoint = new Point(relativePoint.X + contentControl.ActualWidth / 2,
                                          relativePoint.Y + contentControl.ActualHeight / 2);
            Point delta = new Point(middlePoint.X - relativePoint.X, middlePoint.Y - relativePoint.Y);

            Canvas.SetLeft(contentControl, middlePoint.X - delta.Y);
            Canvas.SetTop(contentControl, middlePoint.Y - delta.X);

            double temp = contentControl.Width;
            contentControl.Width = contentControl.Height;
            contentControl.Height = temp;
        }

        /// <summary>
        ///     Sends the currently selected region as white pixels to the server before sending the rotated pixels
        /// </summary>
        private void FillRegionBeforeRotation()
        {
            Point upperLeft = CropWriteableBitmapPosition;
            Point bottomRight = new Point(CropWriteableBitmapPosition.X + CropWriteableBitmap.Width,
                                          CropWriteableBitmapPosition.Y + CropWriteableBitmap.Height);

            OnDrewPixels(this, FillRegionWhite(upperLeft, bottomRight));
        }

        /// <summary>
        ///     Sends updated content of currently selected region
        /// </summary>
        private void UpdateModifiedRegion()
        {
            Point upperLeft = CropWriteableBitmapPosition;
            Point bottomRight = new Point(CropWriteableBitmapPosition.X + CropWriteableBitmap.Width,
                                          CropWriteableBitmapPosition.Y + CropWriteableBitmap.Height);

            OnDrewPixels(this,
                         GetRegionPixels(upperLeft, bottomRight, (int) CropWriteableBitmapPosition.X,
                                         (int) CropWriteableBitmapPosition.Y));
        }

        /// <summary>
        ///     Generates white pixels for the region delimited by the two points
        /// </summary>
        /// <param name="upperLeft">Upper left limit</param>
        /// <param name="bottomRight">Bottom right limit</param>
        /// <returns></returns>
        private static List<Tuple<Point, string>> FillRegionWhite(Point upperLeft, Point bottomRight)
        {
            List<Tuple<Point, string>> whitePixels = new List<Tuple<Point, string>>();

            int upperLeftX = (int) upperLeft.X > 0 ? (int) upperLeft.X : 0;
            int upperLeftY = (int) upperLeft.Y > 0 ? (int) upperLeft.Y : 0;

            int bottomRightX = (int) bottomRight.X > 0 ? (int) bottomRight.X : 0;
            int bottomRightY = (int) bottomRight.Y > 0 ? (int) bottomRight.Y : 0;

            for (int i = upperLeftX; i < bottomRightX; i++)
            {
                for (int j = upperLeftY; j < bottomRightY; j++)
                {
                    whitePixels.Add(new Tuple<Point, string>(new Point(i, j), Colors.White.ToString()));
                }
            }

            return whitePixels;
        }

        /// <summary>
        ///     Gets the actual pixels of delimited region
        /// </summary>
        /// <param name="upperLeft">Upper left limit</param>
        /// <param name="bottomRight">Bottom left limit</param>
        /// <param name="xOffset">CropWriteableBitmap X-Offset (X position)</param>
        /// <param name="yOffset">CropWriteableBitmap Y-Offset (Y position)</param>
        /// <returns></returns>
        private List<Tuple<Point, string>> GetRegionPixels(Point upperLeft, Point bottomRight, int xOffset, int yOffset)
        {
            List<Tuple<Point, string>> pixels = new List<Tuple<Point, string>>();

            int upperLeftX = (int) upperLeft.X > 0 ? (int) upperLeft.X : 0;
            int upperLeftY = (int) upperLeft.Y > 0 ? (int) upperLeft.Y : 0;

            int bottomRightX = (int) bottomRight.X > 0 ? (int) bottomRight.X : 0;
            int bottomRightY = (int) bottomRight.Y > 0 ? (int) bottomRight.Y : 0;

            for (int i = upperLeftX; i < bottomRightX; i++)
            {
                for (int j = upperLeftY; j < bottomRightY; j++)
                {
                    pixels.Add(new Tuple<Point, string>(new Point(i, j),
                                                        CropWriteableBitmap
                                                            .GetPixel(i - xOffset, j - yOffset).ToString()));
                }
            }

            return pixels;
        }

        internal void VerticalFlip(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Flip(WriteableBitmapExtensions.FlipMode.Vertical);
            UpdateModifiedRegion();
        }

        internal void HorizontalFlip(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
            UpdateModifiedRegion();
        }

        internal void GrayFilter(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Gray();
            UpdateModifiedRegion();
        }

        internal void InvertFilter(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Invert();
            UpdateModifiedRegion();
        }

        internal void GaussianBlurFilter(object obj)
        {
            Blur(BlurRadius, BlurIntensity);
            UpdateModifiedRegion();
        }

        internal void Blur(int radius, int intensity)
        {
            int[,] kernel;
            switch (radius)
            {
                case 1:
                    kernel = WriteableBitmapExtensions.KernelGaussianBlur3x3;
                    break;
                case 2:
                    kernel = WriteableBitmapExtensions.KernelGaussianBlur5x5;
                    break;
                case 3:
                    kernel = GaussianBlur.KernelGaussianBlur7x7;
                    break;
                case 4:
                    kernel = GaussianBlur.KernelGaussianBlur9x9;
                    break;
                default:
                    kernel = WriteableBitmapExtensions.KernelGaussianBlur3x3;
                    break;
            }

            int i = 0;
            while (i < intensity)
            {
                CropWriteableBitmap = CropWriteableBitmap.Convolute(kernel);
                i++;
            }
        }

        /// <summary>
        ///     Display a cursor in the drawingSurface
        ///     depending of the tool selected by the user
        /// </summary>
        /// <param name="displayArea"> Border containing the Canvas </param>
        public void PixelCursor(Border displayArea)
        {
            switch (SelectedTool)
            {
                case "pencil":
                    displayArea.Cursor = Cursors.Pen;
                    break;
                case "eraser":
                    Cursor eraser =
                        new Cursor(Application.GetResourceStream(new Uri("/Resources/Cursors/Eraser.cur",
                                                                         UriKind.Relative)).Stream);
                    displayArea.Cursor = eraser;
                    break;
                case "selector":
                    displayArea.Cursor = Cursors.Cross;
                    break;
                case "fill":
                    Cursor fill =
                        new Cursor(Application.GetResourceStream(new Uri("/Resources/Cursors/filler.cur",
                                                                         UriKind.Relative)).Stream);
                    displayArea.Cursor = fill;
                    break;
                default:
                    displayArea.Cursor = Cursors.Pen;
                    break;
            }
        }

        protected void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnDrewPixels(object sender, List<Tuple<Point, string>> drawnPixels)
        {
            DrewPixelsEvent?.Invoke(sender, drawnPixels);
        }

        protected void OnModifiedRegion(Rect modifiedRegion)
        {
            ModifiedRegionEvent?.Invoke(this, modifiedRegion);
        }

        /// <summary>
        ///     The active tool becomes the one passed in parameter
        /// </summary>
        /// <param name="tool"> Tool selected in the canvas </param>
        public void SelectTool(string tool)
        {
            SelectedTool = tool;
            SelectedToolChanged?.Invoke(this, null);
        }

        private static int ToInt(Color mediaColor)
        {
            return (mediaColor.A << AlphaShift) | (mediaColor.R << RedShift) |
                   (mediaColor.G << GreenShift) | mediaColor.B;
        }

        public void FloodFill(Point startPoint, double maxWidth, double maxHeight)
        {
            Color fillColor = (Color) ColorConverter.ConvertFromString(SelectedColor);

            int oldColorInt = ToInt(WriteableBitmap.GetPixel((int) startPoint.X, (int) startPoint.Y));
            int fillColorInt = ToInt(fillColor);

            if (oldColorInt == fillColorInt)
            {
                return;
            }

            Stack<Point> pixels = new Stack<Point>();

            List<Tuple<Point, string>> drawnPixels = new List<Tuple<Point, string>>();

            pixels.Push(startPoint);
            WriteableBitmap.Lock();

            while (pixels.Count != 0)
            {
                Point currentPixel = pixels.Pop();
                int currentY = (int) currentPixel.Y;

                while (0 <= currentY && ToInt(WriteableBitmap.GetPixel((int) currentPixel.X, currentY)) == oldColorInt)
                {
                    currentY--;
                }

                currentY++;

                bool spanLeft = false;
                bool spanRight = false;

                while (0 <= currentY && currentY < maxHeight &&
                       ToInt(WriteableBitmap.GetPixel((int) currentPixel.X, currentY)) == oldColorInt)
                {
                    WriteableBitmap.SetPixel((int) currentPixel.X, currentY, fillColor);
                    drawnPixels.Add(new Tuple<Point, string>(new Point(currentPixel.X, currentY), SelectedColor));

                    if (1.0 < currentPixel.X)
                    {
                        int getColor = ToInt(WriteableBitmap.GetPixel((int) currentPixel.X - 1, currentY));

                        if (!spanLeft && getColor == oldColorInt)
                        {
                            pixels.Push(new Point(currentPixel.X - 1.0, currentY));
                            spanLeft = true;
                        }
                        else if (getColor != oldColorInt)
                        {
                            spanLeft = false;
                        }
                    }

                    if (currentPixel.X < maxWidth - 1.0)
                    {
                        int getColor = ToInt(WriteableBitmap.GetPixel((int) currentPixel.X + 1, currentY));

                        if (!spanRight && getColor == oldColorInt)
                        {
                            pixels.Push(new Point(currentPixel.X + 1.0, currentY));
                            spanRight = true;
                        }
                        else if (getColor != oldColorInt)
                        {
                            spanRight = false;
                        }
                    }

                    currentY++;
                }
            }

            WriteableBitmap.Unlock();

            OnDrewPixels(this, drawnPixels);
        }
    }
}
