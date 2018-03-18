using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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
        private readonly WriteableBitmap _cropWriteableBitmap = BitmapFactory.New(100, 100);

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

            // Clear the WriteableBitmap with white color
            //WriteableBitmap.Clear(Colors.Red);
            //CropWriteableBitmap.Clear(Colors.Blue);
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
                _writeableBitmap = value;
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

        public void ZoneSelector(Point oldPosition, Point newPosition)
        {
            if (SelectedTool == "selector")
            {
                WriteableBitmap =
                    WriteableBitmap.Resize(100, 300,
                                           WriteableBitmapExtensions.Interpolation.Bilinear);

                //_writeableBitmap = _writeableBitmap.Resize(23, 43, WriteableBitmapExtensions.Interpolation.Bilinear);

                Tools tools = new Tools(WriteableBitmap, oldPosition, newPosition);
                tools.SelectZone();
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
