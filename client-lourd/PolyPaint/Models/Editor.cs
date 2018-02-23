using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Threading;
using PolyPaint.Constants;
using Application = System.Windows.Application;

namespace PolyPaint.Models
{
    /// <summary>
    ///     Modélisation de l'éditeur.
    ///     Contient ses différents états et propriétés ainsi que la logique
    ///     qui régis son fonctionnement.
    /// </summary>
    internal class Editor : INotifyPropertyChanged
    {
        private readonly StrokeCollection _removedStrokesCollection = new StrokeCollection();

        // Couleur des traits tracés par le crayon.
        private string _selectedColor = "Black";

        // Forme de la pointe du crayon
        private string _selectedTip = "ronde";

        // Outil actif dans l'éditeur
        private string _selectedTool = "crayon";

        // Grosseur des traits tracés par le crayon.
        private int _strokeSize = 11;

        public StrokeCollection StrokesCollection = new StrokeCollection();

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
            return StrokesCollection.Count > 0;
        }

        // On retire le trait le plus récent de la surface de dessin et on le place sur une pile.
        public void Stack(object o)
        {
            try
            {
                Stroke trait = StrokesCollection.Last();
                _removedStrokesCollection.Add(trait);
                StrokesCollection.Remove(trait);
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
                Stroke trait = _removedStrokesCollection.Last();
                StrokesCollection.Add(trait);
                _removedStrokesCollection.Remove(trait);
                StrokeAdded(trait);
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
                StrokesCollection.Clear();
                new StrokeCollection(file).ToList().ForEach(stroke => StrokesCollection.Add(stroke));
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
            if (autosave)
            {
                if (StrokesCollection.Count == 0) return;
                path = string.Format(FileExtensionConstants.AutosaveFilePath, drawingName);
            }

            FileStream file = null;
            try
            {
                Directory.CreateDirectory(FileExtensionConstants.AutosaveDirPath);
                file = new FileStream(path, FileMode.Create);
                StrokesCollection.Save(file);
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
    }
}