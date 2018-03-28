using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolyPaint.Constants;
using PolyPaint.Helpers;
using Application = System.Windows.Application;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;

namespace PolyPaint.Models.PixelModels
{
    internal class PixelEditor : INotifyPropertyChanged
    {
        //writeableBitmap used on edition
        private WriteableBitmap _cropWriteableBitmap;

        private Point _cropWriteableBitmapPosition;

        private bool _isWriteableBitmapOnEdition;

        // Size of the pixel trace in our draw
        private int _pixelSize = 5;

        // Pixel color drawn by the pencil
        private string _selectedColor = "Black";

        // Actif tool of the editor
        private string _selectedTool = "pencil";

        private WriteableBitmap _writeableBitmap;

        public PixelEditor()
        {
            //Todo: Resize dynamically with the size of the Canvas
            WriteableBitmap = BitmapFactory.New(1000, 1000);
            WriteableBitmap.Clear(Colors.Transparent);

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
                tools.DrawPixel(PixelSize, "Transparent");
            }
        }

        public void DrawIncomingPixel(int x, int y, string pixelColor)
        {
            WriteableBitmap.SetPixel(x, y, (Color) ColorConverter.ConvertFromString(pixelColor));
        }

        public void SelectZone(Thumb selectedZoneThumb, Point oldPosition, Point newPosition)
        {
            Tools tools = new Tools(WriteableBitmap, oldPosition, newPosition);

            //The first point returns the lowest coordinates
            //The second point returns the highest coordinates
            Tuple<Point, Point> selectedRectangle = tools.SelectCropZone();

            //We crop a second WriteableBitmap that will be edited into the size
            // of our selectedZone
            Rect rect = new Rect(selectedRectangle.Item1, selectedRectangle.Item2);
            CropWriteableBitmap = WriteableBitmap.Crop(rect);

            //We move our croppedwriteableBitmap into the position
            //of the selectedZone
            CropWriteableBitmapPosition = selectedRectangle.Item1;
            Canvas.SetLeft(selectedZoneThumb, CropWriteableBitmapPosition.X);
            Canvas.SetTop(selectedZoneThumb, CropWriteableBitmapPosition.Y);

            //Similar to MsPaint, the zoneSelected of our original writeableBitmap is filled with no color
            WriteableBitmap.FillRectangle((int) selectedRectangle.Item1.X, (int) selectedRectangle.Item1.Y,
                                          (int) selectedRectangle.Item2.X, (int) selectedRectangle.Item2.Y,
                                          Colors.Transparent);

            //We can then start our edition
            IsWriteableBitmapOnEdition = true;
        }

        /// <summary>
        ///     Merge the edited bitmap on the original bitmap (the draw)
        /// </summary>
        public void BlitSelectedZone()
        {
            Rect destinationRectangle = new Rect(CropWriteableBitmapPosition.X, CropWriteableBitmapPosition.Y,
                                                 CropWriteableBitmap.PixelWidth, CropWriteableBitmap.PixelHeight);
            Rect sourceRectangle = new Rect(0, 0, CropWriteableBitmap.PixelWidth, CropWriteableBitmap.PixelHeight);

            WriteableBitmap.Blit(destinationRectangle, CropWriteableBitmap, sourceRectangle);

            IsWriteableBitmapOnEdition = false;
            CropWriteableBitmap.Clear(Colors.Transparent);
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

        public void ExportImagePrompt(object canvas)
        {
            //Todo: add empty draw case

            //cast object as a Canvas (object from command)
            if (canvas is Canvas drawingSurface)
            {
                //then save it as an image
                SaveFileDialog exportImageDialog = new SaveFileDialog
                {
                    Title = "Exporter le dessin",
                    Filter = FileExtensionConstants.ExportImageFilter,
                    AddExtension = true
                };
                if (exportImageDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePathNameExt = Path.GetFullPath(exportImageDialog.FileName);
                    ExportImage(filePathNameExt, drawingSurface);
                }
            }
        }

        public void ExportImage(string filePathNameExt, Canvas drawingSurface)
        {
            FileStream imageStream = null;
            try
            {
                int imageWidth = (int) drawingSurface.ActualWidth;
                int imageHeight = (int) drawingSurface.ActualHeight;
                RenderTargetBitmap imageRender = new RenderTargetBitmap(imageWidth, imageHeight,
                                                                        ImageManipulationConstants.DotsPerInch,
                                                                        ImageManipulationConstants.DotsPerInch,
                                                                        PixelFormats.Pbgra32);
                imageRender.Render(drawingSurface);
                imageStream = new FileStream(filePathNameExt, FileMode.Create);
                BitmapEncoder encoder = null;

                switch (Path.GetExtension(filePathNameExt))
                {
                    case ".png": //png
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".jpg": //jpg or jpeg
                        encoder = new JpegBitmapEncoder();
                        ((JpegBitmapEncoder) encoder).QualityLevel = 100; //maximum jpeg quality
                        break;
                    case ".bmp": //bmp
                        encoder = new BmpBitmapEncoder();
                        break;
                }

                encoder?.Frames.Add(BitmapFrame.Create(imageRender));
                encoder?.Save(imageStream);
                //result message
                UserAlerts.ShowInfoMessage("Image exportée avec succès");
            }
            catch (UnauthorizedAccessException e)
            {
                UserAlerts.ShowErrorMessage("L'accès à ce fichier est interdit");
            }
            catch (Exception e)
            {
                UserAlerts.ShowErrorMessage("Une erreur est survenue");
            }
            finally
            {
                imageStream?.Close();
            }
        }
    }
}
