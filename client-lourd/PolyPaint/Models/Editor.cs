using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Ink;
using PolyPaint.Constants;

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

        public ObservableCollection<string> RecentAutosaves { get; internal set; } = new ObservableCollection<string>();

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

        protected void StrokeAdded(Stroke stroke)
        {
            EditorAddedStroke?.Invoke(this, stroke);
        }

        public void OpenDrawing(object o)
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
                FileStream file = null;
                try
                {
                    file = new FileStream(path, FileMode.Open);
                    StrokeCollection tempStrokesCollection = new StrokeCollection(file);
                    StrokesCollection.Clear();
                    tempStrokesCollection.ToList().ForEach(stroke => StrokesCollection.Add(stroke));
                }
                catch
                {
                    //ignored
                    // TODO: Handle exception
                }
                finally
                {
                    file?.Close();
                }
            }
        }


        public void OpenAutosave(object obj)
        {
            throw new NotImplementedException();
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

        public void SaveDrawing(string savePath, bool autosave)
        {
            string path = savePath;
            if (autosave)
                path = FileExtensionConstants.AutosavePath + "DrawingName_autosave" + "." +
                       FileExtensionConstants.DefaultExt;

            FileStream file = null;
            try
            {
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

                // TODO: Alert user of error on promt-save
            }
            catch (Exception e)
            {
                // ignored
                // TODO: Handle exception
            }
            finally
            {
                file?.Close();
            }
        }

        public void UpdateRecentAutosaves()
        {
            try
            {
                if (!Directory.Exists(FileExtensionConstants.AutosavePath))
                    return;

                string[] autosavedDrawings = Directory.GetFiles(FileExtensionConstants.AutosavePath);

                foreach (string filePath in autosavedDrawings) ProcessRecentFile(filePath);
            }
            catch (Exception e)
            {
                var something = e;
                //ignored
            }
        }

        private void ProcessRecentFile(string filePath)
        {
            string fileName = Regex.Match(filePath, "[\\w]*.tide").Value;
            string drawingName = Regex.Replace(fileName, "_[a-z]*.tide", "");
            RecentAutosaves.Add(drawingName);
        }
    }
}