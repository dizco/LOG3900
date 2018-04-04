using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        // original draw
        private WriteableBitmap _writeableBitmap;

        // writeableBitmap used on edition
        private WriteableBitmap _cropWriteableBitmap;

        // temporay writeableBitmap used for a better collab
        private WriteableBitmap _tempWriteableBitmap;

        private Point _cropWriteableBitmapPosition;

        private bool _isWriteableBitmapOnEdition;

        // Size of the pixel trace in our draw
        private int _pixelSize = 5;

        // Pixel color drawn by the pencil
        private string _selectedColor = "Black";

        // Actif tool of the editor
        private string _selectedTool = "pencil";

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

        public ObservableCollection<string> RecentAutosaves { get; } = new ObservableCollection<string>();

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

        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeCropWriteableBitmapPosition(Point position)
        {
            CropWriteableBitmapPosition = position;
        }

        public event EventHandler<List<Tuple<Point, string>>> DrewLineEvent;

        /// <summary>
        ///     Draw on the writeableBitmap
        ///     Transparant color is put on the bitmap when the eraser is selected
        /// </summary>
        /// <param name="oldPosition"> Position of the mouse before an action </param>
        /// <param name="newPosition"> Position of the mouse after the actions </param>
        public void DrawPixels(Point oldPosition, Point newPosition)
        {
            Tools tools = new Tools(WriteableBitmap, oldPosition, newPosition);
            tools.DrewLineEvent += OnDrewLine;
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
        ///     Merge the edited bitmap on the original bitmap (the draw)
        /// </summary>
        /// <param name="selectionContentControl">Control of the zone Selected</param>
        /// <param name="writeableBitmapSource">Writeablebitmap source that is blit on the original draw</param>
        /// <param name="isSelectionOver">the selection ends if the value is set to True</param>
        public void BlitDraw(ContentControl selectionContentControl, WriteableBitmap writeableBitmapSource, bool isSelectionOver)
        {
            if (IsWriteableBitmapOnEdition)
            {
                writeableBitmapSource = writeableBitmapSource.Resize((int)selectionContentControl.Width, (int)selectionContentControl.Height,
                                                                     WriteableBitmapExtensions.Interpolation.NearestNeighbor);

                Point position = new Point(Canvas.GetLeft(selectionContentControl),
                                           Canvas.GetTop(selectionContentControl));

                Rect destinationRectangle = new Rect(position.X, position.Y,
                                                     writeableBitmapSource.PixelWidth,
                                                     writeableBitmapSource.PixelHeight);
                Rect sourceRectangle =
                    new Rect(0, 0, writeableBitmapSource.PixelWidth, writeableBitmapSource.PixelHeight);

                WriteableBitmap.Blit(destinationRectangle, writeableBitmapSource, sourceRectangle);


                if (isSelectionOver)
                {
                    writeableBitmapSource.Clear(Colors.Transparent);
                    TempWriteableBitmap.Clear(Colors.Transparent);
                    IsWriteableBitmapOnEdition = false;
                }
            }
        }


        internal void QuarterTurnClockwise(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Rotate(ClockwiseAngle);
        }

        internal void QuarterTurnCounterClockwise(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Rotate(CounterClockwiseAngle);
        }

        internal void VerticalFlip(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Flip(WriteableBitmapExtensions.FlipMode.Vertical);
        }

        internal void HorizontalFlip(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
        }

        internal void GrayFilter(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Gray();
        }

        internal void InvertFilter(object obj)
        {
            CropWriteableBitmap = CropWriteableBitmap.Invert();
        }

        internal void GaussianBlurFilter(object obj)
        {
            int[,] kernel = WriteableBitmapExtensions.KernelGaussianBlur3x3;
            CropWriteableBitmap = CropWriteableBitmap.Convolute(kernel);
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

        protected void OnDrewLine(object sender, List<Tuple<Point, string>> drawnPixels)
        {
            DrewLineEvent?.Invoke(sender, drawnPixels);
        }

        /// <summary>
        ///     The active tool becomes the one passed in parameter
        /// </summary>
        /// <param name="tool"> Tool selected in the canvas </param>
        public void SelectTool(string tool)
        {
            SelectedTool = tool;
        }

        private static int ToInt(Color mediaColor)
        {
            return (mediaColor.A << AlphaShift) | (mediaColor.R << RedShift) |
                   (mediaColor.G << GreenShift) | mediaColor.B;
        }

        public void FloodFill(Point startPoint, double maxWidth, double maxHeight)
        {
            Color fillColor = (Color) ColorConverter.ConvertFromString(SelectedColor);

            int oldColor = ToInt(WriteableBitmap.GetPixel((int) startPoint.X, (int) startPoint.Y));
            int fillColorInt = ToInt(fillColor);

            if (oldColor == fillColorInt)
            {
                return;
            }

            Stack<Point> pixels = new Stack<Point>();
            pixels.Push(startPoint);
            WriteableBitmap.Lock();

            while (pixels.Count != 0)
            {
                Point currentPixel = pixels.Pop();
                int currentY = (int) currentPixel.Y;

                while (0 <= currentY && ToInt(WriteableBitmap.GetPixel((int) currentPixel.X, currentY)) == oldColor)
                {
                    currentY--;
                }

                currentY++;

                bool spanLeft = false;
                bool spanRight = false;

                while (0 <= currentY && currentY < maxHeight &&
                       ToInt(WriteableBitmap.GetPixel((int) currentPixel.X, currentY)) == oldColor)
                {
                    WriteableBitmap.SetPixel((int) currentPixel.X, currentY, fillColor);

                    if (1.0 < currentPixel.X)
                    {
                        int getColor = ToInt(WriteableBitmap.GetPixel((int) currentPixel.X - 1, currentY));

                        if (!spanLeft && getColor == oldColor)
                        {
                            pixels.Push(new Point(currentPixel.X - 1.0, currentY));
                            spanLeft = true;
                        }
                        else if (getColor != oldColor)
                        {
                            spanLeft = false;
                        }
                    }

                    if (currentPixel.X < maxWidth - 1.0)
                    {
                        int getColor = ToInt(WriteableBitmap.GetPixel((int) currentPixel.X + 1, currentY));

                        if (!spanRight && getColor == oldColor)
                        {
                            pixels.Push(new Point(currentPixel.X + 1.0, currentY));
                            spanRight = true;
                        }
                        else if (getColor != oldColor)
                        {
                            spanRight = false;
                        }
                    }

                    currentY++;
                }
            }

            WriteableBitmap.Unlock();
        }
    }
}
