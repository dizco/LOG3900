using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Threading;
using PolyPaint.Constants;
using PolyPaint.Helpers;
using PolyPaint.Models;
using PolyPaint.Models.ApiModels;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Views;
using Application = System.Windows.Application;

namespace PolyPaint.ViewModels
{
    internal class HomeMenuViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly HomeMenuModel _homeMenu;

        public HomeMenuViewModel()
        {
            _homeMenu = new HomeMenuModel();
            _homeMenu.NewDrawingCreated += DrawingLoadedHandler;
            _homeMenu.OnlineDrawingJoined += OnlineDrawingLoadedHandler;
            FilteredDrawings = _homeMenu.FilteredDrawings;
            AutosavedDrawings = _homeMenu.AutosavedDrawings;

            GoToNewDrawingSubMenuCommand = new RelayCommand<object>(OpenNewDrawingSubMenu);
            StartNewDrawing = new RelayCommand<object>(CreateNewDrawing);
            GoToLocalDrawingSubMenuCommand = new RelayCommand<object>(OpenLocalDrawingSubMenu);
            OpenAutosaveDrawingCommand = new RelayCommand<object>(OpenAutosaveDrawing);
            OpenDrawingPromtCommand = new RelayCommand<object>(OpenDrawingPrompt);
            GoToOnlineDrawingSubMenuCommand = new RelayCommand<object>(OpenOnlineDrawingSubMenu, IsOnline);
            JoinDrawingCommand = new RelayCommand<object>(JoinOnlineDrawing);
            GalleryCommand = new RelayCommand<object>(OpenGallery);
            GoToMenuCommand = new RelayCommand<object>(OpenMenu);
            BackToLogin = new RelayCommand<object>(OpenLogin);
            WebSocketConnectedEvent += (s, a) => RefreshHomeMenuBindings();

            if (string.IsNullOrEmpty(Username))
            {
                LoginButtonVisibility = Visibility.Visible;
            }
        }

        public Visibility MainMenuVisibility { get; set; } = Visibility.Visible;
        public Visibility NewDrawingVisibility { get; set; } = Visibility.Collapsed;
        public Visibility JoinDrawingVisibility { get; set; } = Visibility.Collapsed;
        public Visibility LoginButtonVisibility { get; set; } = Visibility.Hidden;
        public Visibility LocalDrawingVisibility { get; set; } = Visibility.Hidden;
        public string SelectedEditingMode { get; set; }
        public Array EditingModes => Enum.GetValues(typeof(EditingModeOption));

        public ObservableCollection<OnlineDrawingModel> FilteredDrawings { get; set; }

        public ObservableCollection<string> AutosavedDrawings { get; set; }

        public string DrawingSearchTerms
        {
            set => _homeMenu.SearchTextChangedHandlers(value.ToLower());
        }

        public string NewDrawingName { get; set; }

        public OnlineDrawingModel SelectedOnlineDrawing { get; set; }
        public string SelectedAutosaved { get; set; }

        public RelayCommand<object> GalleryCommand { get; set; }
        public RelayCommand<object> GoToOnlineDrawingSubMenuCommand { get; set; }
        public RelayCommand<object> JoinDrawingCommand { get; set; }
        public RelayCommand<object> GoToNewDrawingSubMenuCommand { get; set; }
        public RelayCommand<object> StartNewDrawing { get; set; }
        public RelayCommand<object> GoToLocalDrawingSubMenuCommand { get; set; }
        public RelayCommand<object> OpenDrawingPromtCommand { get; set; }
        public RelayCommand<object> OpenAutosaveDrawingCommand { get; set; }
        public RelayCommand<object> GoToMenuCommand { get; set; }
        public RelayCommand<object> BackToLogin { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ClosingRequest;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Raises ClosingRequest to trigger the closing of the LoginWindowView
        /// </summary>
        private void OnClosingRequest()
        {
            ClosingRequest?.Invoke(this, EventArgs.Empty);
        }

        private static void RefreshHomeMenuBindings()
        {
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(CommandManager
                                                                                         .InvalidateRequerySuggested);
        }

        private bool IsOnline(object obj)
        {
            return Messenger?.IsConnected ?? false;
        }

        private void OpenGallery(object obj)
        {
            // TODO: Create gallery and link it here
            UserAlerts.ShowErrorMessage("Is this the Krusty Krab?");
        }

        private void OpenLogin(object obj)
        {
            if (LoginWindow == null)
            {
                LoginWindow = new LoginWindowView();
                LoginWindow.Show();
                LoginWindow.Closed += (s, a) => LoginWindow = null;
                OnClosingRequest();
            }
        }

        private void OpenDrawingPrompt(object obj)
        {
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
                    StrokeCollection strokes = new StrokeCollection();
                    new StrokeCollection(file).ToList().ForEach(stroke => strokes.Add(stroke));

                    string drawingFileName = Regex.Match(path, "[\\w]*.tide").Value;
                    DrawingName = Regex.Replace(drawingFileName, ".tide", "");

                    OpenEditorWindow(strokes);
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
        }

        private void OpenAutosaveDrawing(object obj)
        {
            if (SelectedAutosaved != null)
            {
                OpenEditorWindow(SelectedAutosaved);
            }
        }

        private void OpenNewDrawingSubMenu(object obj)
        {
            MainMenuVisibility = Visibility.Collapsed;
            NewDrawingVisibility = Visibility.Visible;
            JoinDrawingVisibility = Visibility.Collapsed;
            UpdateVisibilityProperties();
        }

        private void CreateNewDrawing(object obj)
        {
            if (string.IsNullOrWhiteSpace(NewDrawingName))
            {
                UserAlerts.ShowErrorMessage("Le nom ne peut pas être vide.");
                return;
            }

            if (!Enum.TryParse(SelectedEditingMode, out EditingModeOption selectedMode))
            {
                UserAlerts.ShowErrorMessage("Vous devez choisir un mode d'édition.");
                return;
            }

            switch (selectedMode)
            {
                case EditingModeOption.Trait: break;
                case EditingModeOption.Pixel:
                    UserAlerts.ShowErrorMessage("Ce mode n'est pas encore supporté.");
                    return;
                default: return;
            }

            _homeMenu.CreateNewDrawing(NewDrawingName, selectedMode);
        }

        private void OpenMenu(object obj)
        {
            MainMenuVisibility = Visibility.Visible;
            NewDrawingVisibility = Visibility.Collapsed;
            JoinDrawingVisibility = Visibility.Collapsed;
            LocalDrawingVisibility = Visibility.Collapsed;
            UpdateVisibilityProperties();
        }

        private void OpenOnlineDrawingSubMenu(object obj)
        {
            _homeMenu.LoadOnlineDrawingsList();
            MainMenuVisibility = Visibility.Collapsed;
            NewDrawingVisibility = Visibility.Collapsed;
            JoinDrawingVisibility = Visibility.Visible;
            LocalDrawingVisibility = Visibility.Collapsed;
            UpdateVisibilityProperties();
        }

        private void OpenLocalDrawingSubMenu(object obj)
        {
            _homeMenu.LoadAutosavedDrawingsList();
            MainMenuVisibility = Visibility.Collapsed;
            NewDrawingVisibility = Visibility.Collapsed;
            JoinDrawingVisibility = Visibility.Collapsed;
            LocalDrawingVisibility = Visibility.Visible;
            UpdateVisibilityProperties();
        }

        private void JoinOnlineDrawing(object obj)
        {
            if (SelectedOnlineDrawing == null)
            {
                UserAlerts.ShowErrorMessage("Veuillez choisir un dessin");
                return;
            }

            _homeMenu.JoinOnlineDrawing(SelectedOnlineDrawing.Id);
        }

        private void UpdateVisibilityProperties()
        {
            NewDrawingName = string.Empty;
            OnPropertyChanged("DrawingSearchTerms");
            OnPropertyChanged("MainMenuVisibility");
            OnPropertyChanged("NewDrawingVisibility");
            OnPropertyChanged("JoinDrawingVisibility");
            OnPropertyChanged("LocalDrawingVisibility");
        }

        private void DrawingLoadedHandler(object sender, Tuple<string, string, EditingModeOption> drawingParams)
        {
            DrawingRoomId = drawingParams.Item1;
            DrawingName = drawingParams.Item2;
            OpenEditorWindow(drawingParams.Item3);
        }

        private void OnlineDrawingLoadedHandler(object sender,
            Tuple<string, string, EditingModeOption, List<EditorActionModel>> drawingParams)
        {
            DrawingRoomId = drawingParams.Item1;
            DrawingName = drawingParams.Item2;
            OpenEditorWindow(drawingParams.Item3, drawingParams.Item4);
        }

        private void OpenEditorWindow(EditingModeOption option = EditingModeOption.Trait,
            List<EditorActionModel> actions = null)
        {
            // TODO: Use EditingModeOption to open the proper editor
            if (EditorWindow == null)
            {
                EditorWindow = new DrawingWindow();
                EditorWindow.Show();
                // TODO: Modify this function once server saving protocol is established
                (EditorWindow.DataContext as EditorViewModel)?.ReplayActions(actions);
                EditorWindow.Closed += OnEditorClosedHandler;
                OnClosingRequest();
            }
        }

        private void OpenEditorWindow(StrokeCollection strokes)
        {
            if (EditorWindow == null)
            {
                EditorWindow = new DrawingWindow();
                EditorWindow.Show();
                if (EditorWindow.DataContext is EditorViewModel editorViewModel)
                {
                    foreach (Stroke stroke in strokes)
                    {
                        editorViewModel.StrokesCollection.Add(stroke);
                    }
                }

                EditorWindow.Closed += OnEditorClosedHandler;
                OnClosingRequest();
            }
        }

        private void OpenEditorWindow(string autosaveDrawingName)
        {
            if (EditorWindow == null)
            {
                EditorWindow = new DrawingWindow();
                EditorWindow.Show();
                if (EditorWindow.DataContext is EditorViewModel editorViewModel)
                {
                    editorViewModel.LoadAutosaved.Execute(autosaveDrawingName);
                }

                EditorWindow.Closed += OnEditorClosedHandler;
                OnClosingRequest();
            }
        }

        private void OnEditorClosedHandler(object sender, EventArgs e)
        {
            if (HomeMenu == null)
            {
                HomeMenu = new HomeMenu();
                HomeMenu.Show();
                HomeMenu.Closed += (s, a) => HomeMenu = null;
                OnClosingRequest();
            }

            EditorWindow = null;
        }
    }
}
