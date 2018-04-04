using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PolyPaint.CustomComponents;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Strategy.StrokeEditorActionStrategy;

namespace PolyPaint.ViewModels
{
    /// <summary>
    ///     Sert d'intermédiaire entre la vue et le modèle.
    ///     Expose des commandes et propriétés connectées au modèle aux des éléments de la vue peuvent se lier.
    ///     Reçoit des avis de changement du modèle et envoie des avis de changements à la vue.
    /// </summary>
    internal class StrokeEditorViewModel : EditorViewModelBase
    {
        private readonly StrokeEditor _editor = new StrokeEditor();

        private Visibility _chatDocked = Visibility.Collapsed;

        /// <summary>
        ///     Constructeur de VueModele
        ///     On récupère certaines données initiales du modèle et on construit les commandes
        ///     sur lesquelles la vue se connectera.
        /// </summary>
        public StrokeEditorViewModel()
        {
            SubscribeDrawingRoom();
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, EditorPropertyModified est appelée.
            _editor.PropertyChanged += EditorPropertyModified;
            _editor.EditorAddedStroke += OnStrokeCollectedHandler;
            _editor.StrokeStackedEvent += OnStrokeStackedHandler;
            _editor.DrawingName = DrawingName;
            _editor.StrokesCollection.StrokesChanged += StrokesCollectionOnStrokesChanged;
            _editor.SelectedStrokesTransformedEvent += SelectedStrokesTransformed;
            _editor.ResetStrokeActionReceived += ResetStrokeActionReceived;

            // On initialise les attributs de dessin avec les valeurs de départ du modèle.
            DrawingAttributes = new DrawingAttributes
            {
                Color = (Color) ColorConverter.ConvertFromString(_editor.SelectedColor)
            };
            AdjustTip();

            StrokesCollection = _editor.StrokesCollection;
            LockedStrokes = _editor.LockedStrokes;

            // Pour chaque commande, on effectue la liaison avec des méthodes du modèle.
            Stack = new RelayCommand<object>(_editor.Stack, _editor.CanStack);
            Unstack = new RelayCommand<object>(_editor.Unstack, _editor.CanUnstack);
            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChooseTip = new RelayCommand<string>(_editor.SelectTip);
            ChooseTool = new RelayCommand<string>(_editor.SelectTool);
            ChooseShape = new RelayCommand<StrokeEditor.DrawableShapes>(_editor.SelectShape);
            ResetDrawingCommand = new RelayCommand<object>(ResetDrawing);
            OpenFileCommand = new RelayCommand<object>(_editor.OpenDrawingPrompt);
            SaveFileCommand = new RelayCommand<object>(_editor.SaveDrawingPrompt);
            AutosaveFileCommand = new RelayCommand<object>(AutosaveFile);
            LoadAutosaved = new RelayCommand<string>(_editor.OpenAutosave);

            ExportImageCommand = new RelayCommand<InkCanvas>(ExportImagePrompt);

            StrokesCollection.StrokesChanged += AutosaveOnStrokeCollectionChanged;

            //Outgoing editor actions
            SendNewStrokeCommand = new RelayCommand<Stroke>(SendNewStroke);

            //Strokes Rotate tool
            QuarterTurnClockwiseCommand = new RelayCommand<InkCanvas>(_editor.QuarterTurnClockwise);
            QuarterTurnCounterClockwiseCommand = new RelayCommand<InkCanvas>(_editor.QuarterTurnCounterClockwise);
            VerticalFlipCommand = new RelayCommand<InkCanvas>(_editor.VerticalFlip);
            HorizontalFlipCommand = new RelayCommand<InkCanvas>(_editor.HorizontalFlip);

            InsertTextCommand = new RelayCommand<InkCanvas>(InsertText);

            if (Messenger?.IsConnected ?? false)
            {
                ChatDocked = Visibility.Visible;
            }

            LoginStatusChanged += ProcessLoginStatusChange;

            StrokeEditorActionReceived += ProcessReceivedStrokeEditorAction;

            ChangeEditorChatDisplayState += ChatDisplayStateChanged;
        }

        public StrokeEditorViewModel(InkCanvas inkCanvas) : this()
        {
            Canvas = inkCanvas;
        }

        public Visibility ChatDocked
        {
            get => _chatDocked;
            private set
            {
                _chatDocked = value;
                PropertyModified();
            }
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

        public string TextToInsert
        {
            get => _editor.TextToInsertContent;
            set
            {
                _editor.TextToInsertContent = value;
                PropertyModified();
            }
        }

        public string TextSize
        {
            get => _editor.TextToInsertSize;
            set
            {
                _editor.TextToInsertSize = value;
                PropertyModified();
            }
        }

        public StrokeEditor.DrawableShapes ShapeSelected
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
        public ISet<string> LockedStrokes { get; set; }
        private List<string> LockedStrokesHeld { get; set; }

        // Commandes sur lesquels la vue pourra se connecter.
        public RelayCommand<object> Stack { get; set; }

        //Commands for choosing the tools
        public RelayCommand<object> Unstack { get; set; }
        public RelayCommand<string> ChooseTip { get; set; }
        public RelayCommand<string> ChooseTool { get; set; }
        public RelayCommand<StrokeEditor.DrawableShapes> ChooseShape { get; set; }
        public RelayCommand<object> ResetDrawingCommand { get; set; }

        public RelayCommand<object> OpenFileCommand { get; set; }
        public RelayCommand<object> SaveFileCommand { get; set; }
        public RelayCommand<object> AutosaveFileCommand { get; set; }
        public RelayCommand<string> LoadAutosaved { get; set; }

        public RelayCommand<InkCanvas> ExportImageCommand { get; set; }

        //Command for managing the views

        public RelayCommand<object> ShowChatWindowCommand { get; set; }

        //Command for sending editor actions to server
        public RelayCommand<Stroke> SendNewStrokeCommand { get; set; }

        //Strokes Rotate tool
        public RelayCommand<InkCanvas> QuarterTurnClockwiseCommand { get; set; }
        public RelayCommand<InkCanvas> QuarterTurnCounterClockwiseCommand { get; set; }
        public RelayCommand<InkCanvas> VerticalFlipCommand { get; set; }
        public RelayCommand<InkCanvas> HorizontalFlipCommand { get; set; }

        public RelayCommand<InkCanvas> InsertTextCommand { get; set; }

        internal bool IsErasingByPoint { get; set; }
        internal bool IsErasingByStroke { get; set; }

        public new void Dispose()
        {
            base.Dispose();
            StrokeEditorActionReceived -= ProcessReceivedStrokeEditorAction;
            LoginStatusChanged -= ProcessLoginStatusChange;
            ChangeEditorChatDisplayState -= ChatDisplayStateChanged;
        }

        private void ChatDisplayStateChanged(object sender, EditorChatDisplayOptions e)
        {
            switch (e)
            {
                case EditorChatDisplayOptions.Display:
                    ChatDocked = Visibility.Visible;
                    break;
                case EditorChatDisplayOptions.Hide:
                    ChatDocked = Visibility.Collapsed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }
        }

        public event EventHandler<StrokeCollection> LockedStrokesSelectedEvent;

        private void OnStrokeStackedHandler(object sender, CustomStroke stroke)
        {
            Messenger?.SendEditorActionRemoveStroke(stroke);
        }

        private void ProcessLoginStatusChange(object sender, string username)
        {
            _editor.CurrentUsername = username;
        }

        private void ProcessReceivedStrokeEditorAction(object sender, StrokeEditorActionModel e)
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

        private void ResetStrokeActionReceived(object sender, EventArgs args)
        {
            LockedStrokesHeld?.Clear();
        }

        private void SelectedStrokesTransformed(object sender, StrokeCollection strokes)
        {
            OnSelectionTransformedHandler(strokes);
        }

        private void AutosaveOnStrokeCollectionChanged(object sender, StrokeCollectionChangedEventArgs obj)
        {
            AutosaveFileCommand.Execute(string.Empty);
        }

        private void AutosaveFile(object obj)
        {
            _editor.SaveDrawing(string.Empty, true);
        }

        public StrokeCollection AddShape(Point start, Point end)
        {
            return _editor.AddShape(start, end);
        }

        public void InsertText(InkCanvas drawingSurface)
        {
            ToolSelected = "text";

            if (string.IsNullOrWhiteSpace(TextToInsert))
            {
                return;
            }

            TextBox newTextBox = _editor.InsertText(drawingSurface);
            UIElement[] newSelectedText = {newTextBox};
            drawingSurface.Select(newSelectedText);
            TextToInsert = string.Empty;
        }

        public Stroke DrawShape(Point start, Point end)
        {
            return _editor.DrawShape(start, end);
        }

        /// <summary>
        ///     Loads all strokes from the server
        /// </summary>
        /// <param name="strokes">List of strokes to rebuild</param>
        internal void RebuildDrawing(List<StrokeModel> strokes)
        {
            foreach (StrokeModel stroke in strokes)
            {
                CustomStroke newStroke = StrokeHelper.BuildIncomingStroke(stroke, string.Empty);
                _editor.StrokesCollection.Add(newStroke);
            }
        }

        /// <summary>
        ///     Raises an event with the StrokeCocllection containing all the strokes the user does not have access to modify
        /// </summary>
        /// <param name="strokes"></param>
        protected void LockedStrokesSelected(List<Stroke> strokes)
        {
            LockedStrokesSelectedEvent?.Invoke(this, new StrokeCollection(strokes));
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

        internal void ResetDrawing(object o)
        {
            _editor.Reset(o);
            LockedStrokesHeld?.Clear();
            SendResetDrawing();
        }

        /// <summary>
        ///     Handler for InkCanvas event
        /// </summary>
        public void OnStrokeCollectedHandler(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            SendNewStrokeCommand.Execute(e.Stroke);
        }

        /// <summary>
        ///     Handler for Editor event (unstack)
        /// </summary>
        public void OnStrokeCollectedHandler(object sender, Stroke stroke)
        {
            SendNewStrokeCommand.Execute(stroke);
        }

        public void OnStrokeErasingHandler(object sender, InkCanvasStrokeErasingEventArgs e)
        {
            if (LockedStrokes.Contains((e.Stroke as CustomStroke)?.Uuid.ToString()))
            {
                e.Cancel = true;
                return;
            }

            IsErasingByPoint = (sender as CustomRenderingInkCanvas)?.EditingMode == InkCanvasEditingMode.EraseByPoint;
            IsErasingByStroke = (sender as CustomRenderingInkCanvas)?.EditingMode == InkCanvasEditingMode.EraseByStroke;
        }

        internal void OnSelectionChangedHandler(StrokeCollection strokes)
        {
            if (strokes.Count > 0)
            {
                List<Stroke> strokesToRemove =
                    strokes.Where(stroke => LockedStrokes.Contains((stroke as CustomStroke)?.Uuid.ToString())).ToList();

                LockedStrokesSelected(strokesToRemove);

                if (strokesToRemove?.Count > 0)
                {
                    return;
                }

                LockedStrokesHeld = strokes.Select(stroke => (stroke as CustomStroke)?.Uuid.ToString())
                                           .Where(stroke => stroke != null).ToList();
                SendLockStrokes(LockedStrokesHeld);
            }
            else
            {
                SendUnlockStrokes(LockedStrokesHeld);
            }
        }

        private void SendLockStrokes(List<string> lockedStrokesHeld)
        {
            Messenger?.SendEditorActionLockStrokes(lockedStrokesHeld);
        }

        private void SendUnlockStrokes(List<string> lockedStrokesHeld)
        {
            if (!string.IsNullOrEmpty(Messenger?.SendEditorActionUnlockStrokes(lockedStrokesHeld)))
            {
                LockedStrokesHeld.Clear();
            }
        }

        private void StrokesCollectionOnStrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            // User uses erases by point
            if (IsErasingByPoint)
            {
                string removedUuids = (e.Removed.First() as CustomStroke)?.Uuid.ToString();

                // Refreshes UUID values for the new strokes
                foreach (Stroke stroke in e.Added)
                {
                    (stroke as CustomStroke)?.RefreshUuid();
                }

                SendRemoveStroke(new[] {removedUuids}, e.Added);

                IsErasingByPoint = false;
            }
            else if (IsErasingByStroke)
            {
                string removedUuids = (e.Removed.First() as CustomStroke)?.Uuid.ToString();

                SendRemoveStroke(new[] {removedUuids});

                IsErasingByStroke = false;
            }
        }

        /// <summary>
        ///     Replaces the Stroke by a CustomStroke (with UUID) and proceeds to send the stroke to the server.
        /// </summary>
        /// <param name="stroke">Newly added stroke</param>
        private void SendNewStroke(Stroke stroke)
        {
            CustomStroke customStroke =
                stroke is CustomStroke newStroke ? newStroke : _editor.AssignUuidToStroke(stroke);

            SendNewStroke(customStroke);
        }

        private void SendNewStroke(CustomStroke stroke)
        {
            Messenger?.SendEditorActionNewStroke(stroke);
        }

        /// <summary>
        ///     Sends the strokes to be removed/replaced to the server
        /// </summary>
        /// <param name="removed">Collection of strokes to be removed</param>
        /// <param name="added">Collection of strokes to replace the removed strokes</param>
        private void SendRemoveStroke(string[] removed, StrokeCollection added = null)
        {
            Messenger?.SendEditorActionReplaceStroke(removed, added);
        }

        private void SendResetDrawing()
        {
            Messenger?.SendEditorActionResetDrawing();
            EditorActionReplaceStrokeStrategy.DrawingReset();
        }

        private void SubscribeDrawingRoom()
        {
            Messenger?.SubscribeToDrawing();
        }

        public void UnsubscribeDrawingRoom()
        {
            if (LockedStrokesHeld?.Count > 0)
            {
                SendUnlockStrokes(LockedStrokesHeld);
            }

            Messenger?.UnsubscribeToDrawing();
        }

        /// <summary>
        ///     Sends the replacement action for any transformation (move, resize, rotate, flip) to the server
        /// </summary>
        /// <param name="strokes"></param>
        public void OnSelectionTransformedHandler(StrokeCollection strokes)
        {
            Messenger?.SendEditorActionTransformedStrokes(strokes);
        }

        public void ExportImagePrompt(InkCanvas drawingSurface)
        {
            if (StrokesCollection.Count == 0 || _editor._isLoadingDrawing)
            {
                UserAlerts.ShowErrorMessage("Veuillez utiliser un dessin non vide");
                return;
            }

            ExportImagePrompt(this);
        }
    }
}
