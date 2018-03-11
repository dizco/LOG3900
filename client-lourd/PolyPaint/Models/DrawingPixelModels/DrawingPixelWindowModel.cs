using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolyPaint.Constants;
using PolyPaint.Helpers;
using MessageBox = System.Windows.Forms.MessageBox;

namespace PolyPaint.Models
{
    internal class DrawingPixelWindowModel : INotifyPropertyChanged
    {
        private const int ErrorSharingViolation = 32;

        private const int ErrorLockViolation = 33;

        private bool _isLoadingDrawing;

        // Grosseur des traits tracés par le crayon.
        private int _pixelSize = 5;

        // Couleur des traits tracés par le crayon.
        private string _selectedColor = "Black";

        // Outil actif dans l'éditeur
        private string _selectedTool = "pencil";

        public string CurrentUsername { get; set; }

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

        public int StrokeSize
        {
            get => _pixelSize;
            set
            {
                _pixelSize = value;
                PropertyModified();
            }
        }

        public string DrawingName { get; set; }

        public ObservableCollection<string> RecentAutosaves { get; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void DrawPixel(WriteableBitmap writeableBitmap, Point oldPosition, Point newPosition)
        {
            //Todo: Add pencil cursor
            Color Color = (Color) ColorConverter.ConvertFromString(SelectedColor);

            if (SelectedTool == "pencil")
            {
                for (int a = 0; a < StrokeSize; a++)
                {
                    for (int i = 0; i < StrokeSize; i++)
                    {
                        writeableBitmap.DrawLine((int) oldPosition.X + i, (int) oldPosition.Y + a,
                                                 (int) newPosition.X + i, (int) newPosition.Y + a, Color);
                    }
                }
            }

            //waveform.Source = writeableBmp;
        }

        public void ErasePixel(WriteableBitmap writeableBitmap, Point oldPosition, Point newPosition)
        {
            if (SelectedTool == "eraser")
            {
                //Todo: Add Erasor cursor
                for (int a = 0; a < StrokeSize; a++)
                {
                    for (int i = 0; i < StrokeSize; i++)
                    {
                        writeableBitmap.DrawLine((int) oldPosition.X + i, (int) oldPosition.Y + a,
                                                 (int) newPosition.X + i,
                                                 (int) newPosition.Y + a, Colors.Transparent);
                    }
                }
            }
        }

        /// <summary>
        ///     Appelee lorsqu'une propriété d'Editeur est modifiée.
        ///     Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir d'Editeur.
        ///     L'évènement qui contient le nom de la propriété modifiée sera attrapé par VueModele qui pourra
        ///     alors prendre action en conséquence.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // The active tool becomes the one passed in parameter
        public void SelectTool(string outil)
        {
            SelectedTool = outil;
        }

        //Todo: Check if necessary and compatible
        public void OpenDrawingPrompt(object o)
        {
            UpdateRecentAutosaves();
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = FileExtensionConstants.DefaultExt,
                Filter = FileExtensionConstants.Filter
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = Path.GetFullPath(openFileDialog.FileName);
                OpenDrawing(path);
            }
        }

        //Todo: Check if necessary and compatible
        public void OpenAutosave(string drawingName)
        {
            string path = string.Format(FileExtensionConstants.AutosaveFilePath, drawingName);

            OpenDrawing(path);
        }

        //Todo: Check if necessary
        private void OpenDrawing(string filePath)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(filePath, FileMode.Open);
                _isLoadingDrawing = true;
                //new StrokeCollection(file).ToList().ForEach(stroke => StrokesCollection.Add(stroke));
                _isLoadingDrawing = false;
            }
            catch (Exception e)
            {
                UserAlerts.ShowErrorMessage("Une erreure est survenue lors de l'ouverture du fichier. Exception #" +
                                            e.HResult);
            }
            finally
            {
                file?.Close();
            }
        }

        //Todo: Check if necessary
        public void SaveDrawingPrompt(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = FileExtensionConstants.DefaultExt,
                Filter = FileExtensionConstants.Filter
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = Path.GetFullPath(saveFileDialog.FileName);
                SaveDrawing(path, false);
            }
        }

        //Todo: Check if necessary
        public void SaveDrawing(string savePath, bool autosave)
        {
            string path = savePath;

            /*if (StrokesCollection.Count == 0 || _isLoadingDrawing)
            {
                return;
            }
            */
            if (string.IsNullOrWhiteSpace(DrawingName))
            {
                return;
            }

            if (autosave)
            {
                path = string.Format(FileExtensionConstants.AutosaveFilePath, DrawingName);
            }

            FileStream file = null;
            try
            {
                Directory.CreateDirectory(FileExtensionConstants.AutosaveDirPath);
                Task autosaveTask = new Task(() =>
                {
                    try
                    {
                        file = new FileStream(path, FileMode.Create);
                        //StrokesCollection.Save(file);
                    }
                    catch (IOException e)
                    {
                        int errorCode = e.HResult & ((1 << 16) - 1);

                        // If exception is not caused by file being in use, rethrow
                        if (errorCode != ErrorSharingViolation && errorCode != ErrorLockViolation)
                        {
                            throw;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                });
                autosaveTask.Start();
            }
            catch (UnauthorizedAccessException e)
            {
                // ignored
                if (autosave)
                {
                    // TODO: Show error in statusbar on autosave
                }

                UserAlerts.ShowErrorMessage("Impossible d'accéder au fichier ou répertoire. Exception #" + e.HResult);
            }
            catch (Exception e)
            {
                // ignored
                if (!autosave)
                {
                    UserAlerts.ShowErrorMessage("Une erreur est survenue lors de l'ouverture du fichier. Exception #" +
                                                e.HResult);
                }
            }
            finally
            {
                file?.Close();
                UpdateRecentAutosaves();
            }
        }

        //Todo: Get the function compatible
        public void ExportImagePrompt(object canvas)
        {
            //cancels an empty drawing exportation
            /*if (StrokesCollection.Count == 0 || _isLoadingDrawing)
            {
                UserAlerts.ShowErrorMessage("Veuillez utiliser un dessin non vide");
                return;
            }*/

            //cast object as a inkCanvas (object from command)
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

        //Todo: Get the function compatible
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
                ShowUserInfoMessage("Image exportée avec succès");
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

        //Todo: Check if necessary
        public void UpdateRecentAutosaves()
        {
            string[] autosavedDrawings = FetchAutosavedDrawings();

            RecentAutosaves.Clear();
            foreach (string filePath in autosavedDrawings)
            {
                ProcessRecentFile(filePath);
            }
        }

        //Todo: Check if necessary
        internal static string[] FetchAutosavedDrawings()
        {
            try
            {
                if (!Directory.Exists(FileExtensionConstants.AutosaveDirPath))
                {
                    throw new ArgumentException();
                }

                return Directory.GetFiles(FileExtensionConstants.AutosaveDirPath);
            }
            catch
            {
                return null;
            }
        }

        //Todo: Check if necessary
        /// <summary>
        ///     Processes all files in autosave directory
        ///     First RegExp extracts the filename from filepath (%LocalAppData%/Temp/PolyPaintPro/DrawingName_autosave.tide =>
        ///     DrawingName_autosave.tide)
        ///     Second RegExp extracts the drawing name from filename (DrawingName_autosave.tide => DrawingName)
        /// </summary>
        private void ProcessRecentFile(string filePath)
        {
            RecentAutosaves.Add(AutosaveFileNameToString(filePath));
        }

        //Todo: Check if necessary
        internal static string AutosaveFileNameToString(string fileName)
        {
            string name = Regex.Match(fileName, "[\\w]*.tide").Value;
            string drawingName = Regex.Replace(name, "_[a-z]*.tide", "");

            return drawingName;
        }

        private void ShowUserInfoMessage(string message)
        {
            MessageBox.Show(message, @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
