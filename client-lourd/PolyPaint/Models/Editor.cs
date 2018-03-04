using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        private InkCanvas _surfaceDessin;
        /*
         * https://social.msdn.microsoft.com/Forums/vstudio/en-US/7900f5c0-0a95-4d32-af94-e5f152d436c9/use-of-inkcanvas-cantrol-in-mvvm-pattern-to-save-image?forum=wpf
         * https://social.msdn.microsoft.com/Forums/vstudio/en-US/5434a8b4-4c80-47a8-ba44-e63e854e0a08/inkcanvas-save-to-image?forum=wpf
         * https://www.google.ca/search?q=inkcanvas+ExportToPng&rlz=1C1AVNE_enCA705CA705&oq=inkcanvas+ExportToPng+&aqs=chrome..69i57.7711j0j7&sourceid=chrome&ie=UTF-8
         * https://social.msdn.microsoft.com/Forums/vstudio/en-US/ccb3a286-5615-40a0-8b19-854993709df7/inkcanvas-to-image-black-border-on-top?forum=wpf
         */

        private bool _isLoadingDrawing;

        // Couleur des traits tracés par le crayon.
        private string _selectedColor = "Black";

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
            set
            {
                SelectedTool = "crayon";
                _selectedTip = value;
                PropertyModified();
            }
        }

        public string SelectedColor
        {
            get => _selectedColor;
            // Lorsqu'on sélectionne une couleur c'est généralement pour ensuite dessiner un trait.
            // C'est pourquoi lorsque la couleur est changée, l'outil est automatiquement changé pour le crayon.
            set
            {
                _selectedColor = value;
                SelectedTool = "crayon";
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

        public void ExportImagePrompt(object inkCanvasParameter)
        {
            //cancels an empty drawing exportation
            if (StrokesCollection.Count == 0 || _isLoadingDrawing)
            {
                ShowUserErrorMessage("Veuillez utiliser un dessin non vide");
                return;
            }

            //cast object as a inkCanvas (object from command)
            if (inkCanvasParameter is InkCanvas inkCanvas)
            {
                //then save it as image
                _surfaceDessin = inkCanvas;
                SaveFileDialog exportImageDialog = new SaveFileDialog
                {
                    Title = "Exporter le dessin",
                    Filter = FileExtensionConstants.ExportImageFilter,
                    AddExtension = true,
                    DefaultExt = FileExtensionConstants.ExportImgDefaultExt
                };
                if (exportImageDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = Path.GetFullPath(exportImageDialog.FileName);
                    ExportImage(path, exportImageDialog.FileName, exportImageDialog.FilterIndex);
                }
            }
        }

        /*
         *https://blogs.msdn.microsoft.com/saveenr/2008/09/18/wpfxaml-saving-a-window-or-canvas-as-a-png-bitmap/
         *https://www.codeproject.com/Articles/16579/Saving-Rebuilding-InkCanvas-Strokes
         * https://mtaulty.com/2016/02/16/windows-10-uwp-inkcanvas-and-rendertargetbitmap/
         * https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/how-to-data-bind-to-an-inkcanvas
         * cast : https://www.danielcrabtree.com/blog/152/c-sharp-7-is-operator-patterns-you-wont-need-as-as-often
         */
        public void ExportImage(string path, string fileName, int filterIndex)
        {
            double dotsPerInch = 100;
            int imageWidth = (int) _surfaceDessin.ActualWidth;
            int imageHeight = (int)_surfaceDessin.ActualHeight;
            RenderTargetBitmap rtb = new RenderTargetBitmap(imageWidth, imageHeight, dotsPerInch, dotsPerInch, PixelFormats.Pbgra32);
            rtb.Render(_surfaceDessin);

            //Directory.CreateDirectory(FileExtensionConstants.AutosaveDirPath);
            PngBitmapEncoder pbe = new PngBitmapEncoder();
            FileStream file = new FileStream(path, FileMode.Create);
            pbe.Frames.Add(BitmapFrame.Create(rtb));
            //file = File.Create(fileName);
            pbe.Save(file);

            //result message
            ShowUserInfoMessage("Image exportée avec succès");
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
    }
}
