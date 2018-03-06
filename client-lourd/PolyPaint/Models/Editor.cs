using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PolyPaint.Constants;
using PolyPaint.CustomComponents;
using PolyPaint.Helpers.Communication;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace PolyPaint.Models
{
    /// <summary>
    ///     Modélisation de l'éditeur.
    ///     Contient ses différents états et propriétés ainsi que la logique
    ///     qui régis son fonctionnement.
    /// </summary>
    internal class Editor : INotifyPropertyChanged
    {
        private const int ErrorSharingViolation = 32;

        private const int ErrorLockViolation = 33;

        private readonly StrokeCollection _removedStrokesCollection = new StrokeCollection();

        private bool _isLoadingDrawing;

        // Couleur des traits tracés par le crayon.
        private string _selectedColor = "Black";

        // Shape selected
        private DrawableShapes _selectedShape = DrawableShapes.Line;

        // Forme de la pointe du crayon
        private string _selectedTip = "ronde";

        // Outil actif dans l'éditeur
        private string _selectedTool = "crayon";

        // Grosseur des traits tracés par le crayon.
        private int _strokeSize = 11;

        public StrokeCollection StrokesCollection = new StrokeCollection();

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

        public string SelectedTip
        {
            get => _selectedTip;
            //When we select a tip, it is generaly to draw after that a stroke or a shape
            //That's why we the tip is changed, the tool is automaticaly changed for one of both
            set
            {
                _selectedTip = value;
                if (!_selectedTool.Equals("shapes")) SelectedTool = "crayon";

                PropertyModified();
            }
        }

        public DrawableShapes SelectedShape
        {
            get => _selectedShape;
            set
            {
                SelectedTool = "shapes";
                _selectedShape = value;
                PropertyModified();
            }
        }

        public string SelectedColor
        {
            get => _selectedColor;

            //When we select a color, it is generaly to draw after that a stroke or a shape
            //That's why we the color is changed, the tool is automaticaly changed for one of both
            set
            {
                _selectedColor = value;
                if (!_selectedTool.Equals("shapes")) SelectedTool = "crayon";

                PropertyModified();
            }
        }

        public int StrokeSize
        {
            get => _strokeSize;
            // Lorsqu'on sélectionne une taille de trait c'est généralement pour ensuite dessiner un trait.
            // C'est pourquoi lorsque la taille est changée, l'outil est automatiquement changé pour le crayon.
            set
            {
                _strokeSize = value;
                SelectedTool = "crayon";
                PropertyModified();
            }
        }

        public ObservableCollection<string> RecentAutosaves { get; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<Stroke> EditorAddedStroke;
        public event EventHandler<Stroke> StrokeStackedEvent;

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

        // S'il y a au moins 1 trait sur la surface, il est possible d'exécuter Stack.
        public bool CanStack(object o)
        {
            bool authoredOneItem = false;

            int i = 0;
            while (!authoredOneItem && i < StrokesCollection.Count)
                if ((StrokesCollection[i] as CustomStroke)?.Author == null)
                    authoredOneItem = true;
                else
                    i++;

            return authoredOneItem;
        }

        // On retire le trait le plus récent de la surface de dessin et on le place sur une pile.
        public void Stack(object o)
        {

            try
            {
                Stroke toRemove = null;
                foreach (Stroke stroke in StrokesCollection.ToArray().Reverse())
                    if ((stroke as CustomStroke)?.Author == null)
                    {
                        toRemove = stroke;
                        break;
                    }
                if (toRemove != null)
                {
                    _removedStrokesCollection.Add(toRemove);
                    StrokesCollection.Remove(toRemove);
                    StrokeStackedEvent?.Invoke(this, toRemove);
                }
            }
            catch
            {
                // ignored
            }
        }

        // S'il y a au moins 1 trait sur la pile de traits retirés, il est possible d'exécuter Unstack.
        public bool CanUnstack(object o)
        {
            return _removedStrokesCollection.Count > 0;
        }

        // On retire le trait du dessus de la pile de traits retirés et on le place sur la surface de dessin.
        public void Unstack(object o)
        {
            try
            {
                Stroke stroke = _removedStrokesCollection.Last();
                StrokesCollection.Add(stroke);
                _removedStrokesCollection.Remove(stroke);
                StrokeAdded(stroke);
            }
            catch
            {
                // ignored
            }
        }

        // On assigne une nouvelle forme de pointe passée en paramètre.
        public void SelectTip(string pointe)
        {
            SelectedTip = pointe;
        }

        // L'outil actif devient celui passé en paramètre.
        public void SelectTool(string outil)
        {
            SelectedTool = outil;
        }

        //The active tool become the one passed in parameter
        public void SelectShape(DrawableShapes shape)
        {
            SelectedShape = shape;
        }

        //Add a shape in the StrokeCollection
        public StrokeCollection AddShape(Point start, Point end)
        {
            //We add the stroke in our primary StrokeCollection
            Stroke shapeStroke = DrawShape(start, end);
            StrokesCollection.Add(shapeStroke);
            StrokeAdded(shapeStroke);

            //We add the stroke in a temporary StrokeCollection that
            //is going to be selected by the inkCanvas 
            StrokeCollection selectedShape = new StrokeCollection();
            selectedShape.Add(shapeStroke);
            SelectedTool = "lasso";

            return selectedShape;
        }

        //Create a stroke of the shape selected
        public Stroke DrawShape(Point start, Point end)
        {
            ShapeMaker shape = new ShapeMaker(start, end);

            //Create a Vertice as a basic stroke
            Stroke shapeStroke = shape.DrawLine();

            // Draw the correct shape
            switch (_selectedShape)
            {
                case DrawableShapes.Line:
                    shapeStroke = shape.DrawLine();
                    break;

                case DrawableShapes.Triangle:
                    shapeStroke = shape.DrawTriangle();
                    break;

                case DrawableShapes.Arrow:
                    shapeStroke = shape.DrawArrow();
                    break;

                case DrawableShapes.Diamond:
                    shapeStroke = shape.DrawDiamond();
                    break;

                case DrawableShapes.LightningBolt:
                    shapeStroke = shape.DrawLightningBolt();
                    break;

                case DrawableShapes.ITetrimino:
                    shapeStroke = shape.DrawITetromino();
                    break;

                case DrawableShapes.JTetrimino:
                    shapeStroke = shape.DrawJTetromino();
                    break;

                case DrawableShapes.LTetrimino:
                    shapeStroke = shape.DrawLTetromino();
                    break;

                case DrawableShapes.OTetrimino:
                    shapeStroke = shape.DrawOTetromino();
                    break;

                case DrawableShapes.STetrimino:
                    shapeStroke = shape.DrawSTetromino();
                    break;

                case DrawableShapes.TTetrimino:
                    shapeStroke = shape.DrawTTetromino();
                    break;

                case DrawableShapes.ZTetrimino:
                    shapeStroke = shape.DrawZTetromino();
                    break;
            }

            //Giving selected drawing attributes to our shape
            shapeStroke.DrawingAttributes.StylusTip = SelectedTip == "ronde" ? StylusTip.Ellipse : StylusTip.Rectangle;
            shapeStroke.DrawingAttributes.Width = SelectedTip == "verticale" ? 1 : StrokeSize;
            shapeStroke.DrawingAttributes.Height = SelectedTip == "horizontale" ? 1 : StrokeSize;
            shapeStroke.DrawingAttributes.Color = (Color) ColorConverter.ConvertFromString(SelectedColor);
            return shapeStroke;
        }

        // We add a new shape form passed in parameter
        public void SelectShape(string point)
        {
            SelectedTip = point;
        }

        // On vide la surface de dessin de tous ses traits.
        public void Reset(object o)
        {
            StrokesCollection.Clear();
            _removedStrokesCollection.Clear();
        }

        private void StrokeAdded(Stroke stroke)
        {
            EditorAddedStroke?.Invoke(this, stroke);
        }

        internal void AddIncomingStroke(Stroke stroke)
        {
            Dispatcher dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

            dispatcher.Invoke(() => { StrokesCollection.Add(stroke); });
        }

        internal void RemoveStackedStroke(Stroke stackedStroke)
        {
            Dispatcher dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

            dispatcher.Invoke(() =>
            {
                Stroke toRemove = null;

                foreach (Stroke stroke in StrokesCollection.ToArray().Reverse())
                {
                    if (StrokeHelper.AreSameStroke(stackedStroke, stroke))
                    {
                        toRemove = stroke;
                        break;
                    }
                }

                if (toRemove != null) StrokesCollection.Remove(toRemove);
            });
        }

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

        public void OpenAutosave(object drawingName)
        {
            string path = string.Format(FileExtensionConstants.AutosaveFilePath, drawingName);

            OpenDrawing(path);
        }

        private void OpenDrawing(string filePath)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(filePath, FileMode.Open);
                _isLoadingDrawing = true;
                StrokesCollection.Clear();
                new StrokeCollection(file).ToList().ForEach(stroke => StrokesCollection.Add(stroke));
                _isLoadingDrawing = false;
            }
            catch (Exception e)
            {
                //ignored
                // TODO: Handle exception
                ShowUserErrorMessage("Une erreure est survenue lors de l'ouverture du fichier. Exception #" +
                                     e.HResult);
            }
            finally
            {
                file?.Close();
            }
        }

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

        public void SaveDrawing(string savePath, bool autosave, string drawingName = "DrawingName")
        {
            string path = savePath;

            if (StrokesCollection.Count == 0 || _isLoadingDrawing)
                return;

            if (autosave)
                path = string.Format(FileExtensionConstants.AutosaveFilePath, drawingName);

            FileStream file = null;
            try
            {
                Directory.CreateDirectory(FileExtensionConstants.AutosaveDirPath);
                Task autosaveTask = new Task(() =>
                {
                    try
                    {
                        file = new FileStream(path, FileMode.Create);
                        StrokesCollection.Save(file);
                    }
                    catch (IOException e)
                    {
                        int errorCode = e.HResult & ((1 << 16) - 1);

                        // If exception is not caused by file being in use, rethrow
                        if (errorCode != ErrorSharingViolation && errorCode != ErrorLockViolation)
                            throw;
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

                ShowUserErrorMessage("Impossible d'accéder au fichier ou répertoire. Exception #" + e.HResult);
            }
            catch (Exception e)
            {
                // ignored
                if (!autosave)
                    ShowUserErrorMessage("Une erreur est survenue lors de l'ouverture du fichier. Exception #" +
                                         e.HResult);
            }
            finally
            {
                file?.Close();
                UpdateRecentAutosaves();
            }
        }

        public void ExportImagePrompt(object inkCanvas)
        {
            //cancels an empty drawing exportation
            if (StrokesCollection.Count == 0 || _isLoadingDrawing)
            {
                ShowUserErrorMessage("Veuillez utiliser un dessin non vide");
                return;
            }

            //cast object as a inkCanvas (object from command)
            if (inkCanvas is InkCanvas drawingSurface)
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
        
        public void ExportImage(string filePathNameExt, InkCanvas drawingSurface)
        {
            FileStream imageStream = null;
            try
            {
                int imageWidth = (int)drawingSurface.ActualWidth;
                int imageHeight = (int)drawingSurface.ActualHeight;
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
                        ((JpegBitmapEncoder)encoder).QualityLevel = 100;//maximum jpeg quality
                        break;
                    case ".bmp": //bmp
                        encoder = new BmpBitmapEncoder();
                        break;
                }

                encoder.Frames.Add(BitmapFrame.Create(imageRender));
                encoder.Save(imageStream);
                //result message
                ShowUserInfoMessage("Image exportée avec succès");
            }
            catch (UnauthorizedAccessException e)
            {
                ShowUserErrorMessage("L'accès à ce fichier est interdit");
            }
            catch (Exception e)
            {
                ShowUserErrorMessage("Une erreur est survenue");
            }
            finally
            {
                imageStream.Close();
            }
        }

        public void UpdateRecentAutosaves()
        {
            try
            {
                // If directory doesn't exist, no file was ever autosaved
                if (!Directory.Exists(FileExtensionConstants.AutosaveDirPath))
                    return;

                string[] autosavedDrawings = Directory.GetFiles(FileExtensionConstants.AutosaveDirPath);

                RecentAutosaves.Clear();
                foreach (string filePath in autosavedDrawings) ProcessRecentFile(filePath);
            }
            catch
            {
                //ignored
            }
        }

        /// <summary>
        ///     Processes all files in autosave directory
        ///     First RegExp extracts the filename from filepath (%LocalAppData%/Temp/PolyPaintPro/DrawingName_autosave.tide =>
        ///     DrawingName_autosave.tide)
        ///     Second RegExp extracts the drawing name from filename (DrawingName_autosave.tide => DrawingName)
        /// </summary>
        private void ProcessRecentFile(string filePath)
        {
            string fileName = Regex.Match(filePath, "[\\w]*.tide").Value;
            string drawingName = Regex.Replace(fileName, "_[a-z]*.tide", "");
            RecentAutosaves.Add(drawingName);
        }

        private void ShowUserErrorMessage(string message)
        {
            MessageBox.Show(message, @"Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowUserInfoMessage(string message)
        {
            MessageBox.Show(message, @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Drawable Shapes
        internal enum DrawableShapes
        {
            Line,
            Triangle,
            Diamond,
            Arrow,
            LightningBolt,
            ITetrimino,
            JTetrimino,
            LTetrimino,
            OTetrimino,
            STetrimino,
            TTetrimino,
            ZTetrimino
        }
    }
}
