using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PolyPaint.Helpers;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Strategy.EditorActionStrategy;
using PolyPaint.Views;

namespace PolyPaint.ViewModels
{
    /// <summary>
    ///     Sert d'intermédiaire entre la vue et le modèle.
    ///     Expose des commandes et propriétés connectées au modèle aux des éléments de la vue peuvent se lier.
    ///     Reçoit des avis de changement du modèle et envoie des avis de changements à la vue.
    /// </summary>
    internal class EditorViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly Editor _editor = new Editor();

        /// <summary>
        ///     Constructeur de VueModele
        ///     On récupère certaines données initiales du modèle et on construit les commandes
        ///     sur lesquelles la vue se connectera.
        /// </summary>
        public EditorViewModel()
        {
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, EditorPropertyModified est appelée.
            _editor.PropertyChanged += EditorPropertyModified;
            _editor.EditorAddedStroke += OnStrokeCollectedHandler;
            _editor.StrokeStackedEvent += OnStrokeStackedHandler;
            _editor.DrawingName = DrawingName;

            // On initialise les attributs de dessin avec les valeurs de départ du modèle.
            DrawingAttributes = new DrawingAttributes
            {
                Color = (Color) ColorConverter.ConvertFromString(_editor.SelectedColor)
            };
            AdjustTip();

            StrokesCollection = _editor.StrokesCollection;

            // Pour chaque commande, on effectue la liaison avec des méthodes du modèle.
            Stack = new RelayCommand<object>(_editor.Stack, _editor.CanStack);
            Unstack = new RelayCommand<object>(_editor.Unstack, _editor.CanUnstack);
            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChooseTip = new RelayCommand<string>(_editor.SelectTip);
            ChooseTool = new RelayCommand<string>(_editor.SelectTool);
            ChooseShape = new RelayCommand<Editor.DrawableShapes>(_editor.SelectShape);
            ResetDrawing = new RelayCommand<object>(_editor.Reset);

            OpenFileCommand = new RelayCommand<object>(_editor.OpenDrawingPrompt);
            SaveFileCommand = new RelayCommand<object>(_editor.SaveDrawingPrompt);
            AutosaveFileCommand = new RelayCommand<object>(AutosaveFile);
            LoadAutosaved = new RelayCommand<object>(_editor.OpenAutosave);

            ExportImageCommand = new RelayCommand<object>(_editor.ExportImagePrompt);

            StrokesCollection.StrokesChanged += (sender, obj) => { AutosaveFileCommand.Execute(string.Empty); };

            //Outgoing editor actions
            SendNewStrokeCommand = new RelayCommand<Stroke>(SendNewStroke);

            //Managing different View
            ShowLoginWindowCommand = new RelayCommand<object>(ShowLoginWindow);

            EditorActionReceived += ProcessReceivedEditorAction;

            LoginStatusChanged += ProcessLoginStatusChange;
        }

        // Ensemble d'attributs qui définissent l'apparence d'un trait.
        public DrawingAttributes DrawingAttributes { get; set; }

        public string ToolSelected
        {
            get => _editor.SelectedTool;
            set => PropertyModified();
        }

        public string ColorSelected
        {
            get => _editor.SelectedColor;
            set => _editor.SelectedColor = value;
        }

        public string TipSelected
        {
            get => _editor.SelectedTip;
            set => PropertyModified();
        }

        public Editor.DrawableShapes ShapeSelected
        {
            get => _editor.SelectedShape;
            set => PropertyModified();
        }

        public int StrokeSizeSelected
        {
            get => _editor.StrokeSize;
            set => _editor.StrokeSize = value;
        }

        public ObservableCollection<string> RecentAutosaves
        {
            get
            {
                _editor.UpdateRecentAutosaves();
                return _editor.RecentAutosaves;
            }
        }

        public StrokeCollection StrokesCollection { get; set; }

        // Commandes sur lesquels la vue pourra se connecter.
        public RelayCommand<object> Stack { get; set; }

        //Commands for choosing the tools
        public RelayCommand<object> Unstack { get; set; }
        public RelayCommand<string> ChooseTip { get; set; }
        public RelayCommand<string> ChooseTool { get; set; }
        public RelayCommand<Editor.DrawableShapes> ChooseShape { get; set; }
        public RelayCommand<object> ResetDrawing { get; set; }

        public RelayCommand<object> OpenFileCommand { get; set; }
        public RelayCommand<object> SaveFileCommand { get; set; }
        public RelayCommand<object> AutosaveFileCommand { get; set; }
        public RelayCommand<object> LoadAutosaved { get; set; }

        public RelayCommand<object> ExportImageCommand { get; set; }

        //Command for managing the views
        public RelayCommand<object> ShowLoginWindowCommand { get; set; }

        public RelayCommand<object> ShowChatWindowCommand { get; set; }

        //Command for sending editor actions to server
        public RelayCommand<Stroke> SendNewStrokeCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnStrokeStackedHandler(object sender, Stroke stroke)
        {
            Messenger?.SendEditorStrokeStack(stroke);
        }

        private void ProcessLoginStatusChange(object sender, string username)
        {
            _editor.CurrentUsername = username;
        }

        private void ProcessReceivedEditorAction(object sender, EditorActionModel e)
        {
            EditorActionStrategyContext context = new EditorActionStrategyContext(e);
            context.ExecuteStrategy(_editor);

            // OVERKILL
            // Currently the only way to refresh CanExecute bindings
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
            {
                CommandManager
                    .InvalidateRequerySuggested();
            });
        }

        private void AutosaveFile(object obj)
        {
            _editor.SaveDrawing(string.Empty, true);
        }

        public StrokeCollection AddShape(Point start, Point end)
        {
            return _editor.AddShape(start, end);
        }

        public Stroke DrawShape(Point start, Point end)
        {
            return _editor.DrawShape(start, end);
        }

        /// <summary>
        ///     Appelee lorsqu'une propriété de VueModele est modifiée.
        ///     Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir de VueModèle.
        ///     L'évènement qui contient le nom de la propriété modifiée sera attrapé par la vue qui pourra
        ///     alors mettre à jour les composants concernés.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Traite les évènements de modifications de propriétés qui ont été lancés à partir
        ///     du modèle.
        /// </summary>
        /// <param name="sender">L'émetteur de l'évènement (le modèle)</param>
        /// <param name="e">
        ///     Les paramètres de l'évènement. PropertyName est celui qui nous intéresse.
        ///     Il indique quelle propriété a été modifiée dans le modèle.
        /// </param>
        private void EditorPropertyModified(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedColor")
            {
                DrawingAttributes.Color = (Color) ColorConverter.ConvertFromString(_editor.SelectedColor);
            }
            else if (e.PropertyName == "SelectedTool")
            {
                ToolSelected = _editor.SelectedTool;
            }
            else if (e.PropertyName == "SelectedTip")
            {
                TipSelected = _editor.SelectedTip;
                AdjustTip();
            }
            else // e.PropertyName == "StrokeSize"
            {
                AdjustTip();
            }
        }

        /// <summary>
        ///     C'est ici qu'est défini la forme de la pointe, mais aussi sa taille (StrokeSize).
        ///     Pourquoi deux caractéristiques se retrouvent définies dans une même méthode? Parce que pour créer une pointe
        ///     horizontale ou verticale, on utilise une pointe carrée et on joue avec les tailles pour avoir l'effet désiré.
        /// </summary>
        private void AdjustTip()
        {
            DrawingAttributes.StylusTip = _editor.SelectedTip == "ronde" ? StylusTip.Ellipse : StylusTip.Rectangle;
            DrawingAttributes.Width = _editor.SelectedTip == "verticale" ? 1 : _editor.StrokeSize;
            DrawingAttributes.Height = _editor.SelectedTip == "horizontale" ? 1 : _editor.StrokeSize;
        }

        //Show login window
        public void ShowLoginWindow(object o)
        {
            if (Messenger?.IsConnected == true)
            {
                if (ChatWindow == null)
                {
                    ChatWindow = new ChatWindowView();
                    ChatWindow.Show();
                    ChatWindow.Closed += (sender, args) => ChatWindow = null;
                }
                else
                {
                    ChatWindow.Activate();
                }
            }
            else if (LoginWindow == null)
            {
                LoginWindow = new LoginWindowView();
                LoginWindow.Show();
                LoginWindow.Closed += LoginViewClosed;
            }
            else
            {
                LoginWindow.Activate();
            }
        }

        private void LoginViewClosed(object sender, EventArgs e)
        {
            LoginWindow = null;
        }

        /// <summary>
        ///     Handler for InkCanvas events
        /// </summary>
        public void OnStrokeCollectedHandler(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            SendNewStrokeCommand.Execute(e.Stroke);
        }

        /// <summary>
        ///     Handler for Editeur events (depilage)
        /// </summary>
        public void OnStrokeCollectedHandler(object sender, Stroke stroke)
        {
            SendNewStrokeCommand.Execute(stroke);
        }

        private void SendNewStroke(Stroke stroke)
        {
            Messenger?.SendEditorActionNewStroke(stroke);
        }
    }
}
