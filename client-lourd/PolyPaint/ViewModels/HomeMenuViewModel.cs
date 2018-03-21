using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
    internal class HomeMenuViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private readonly HomeMenuModel _homeMenu;

        private bool _createPasswordProtectedDrawing;
        private bool _createPubliclyVisibleDrawing = true;

        public HomeMenuViewModel()
        {
            _homeMenu = new HomeMenuModel();
            _homeMenu.NewDrawingCreated += DrawingLoadedHandler;
            _homeMenu.OnlineDrawingJoined += OnlineDrawingLoadedHandler;
            _homeMenu.OnlineDrawingJoinFailed += OnlineDrawingLoadingFailedHandler;
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

            ToggleNewDrawingProtection = new RelayCommand<object>(ToggleProtection);
            ToggleNewDrawingVisibility = new RelayCommand<object>(ToggleDrawingVisibility);

            WebSocketConnectedEvent += RefreshHomeMenuBindings;
            WebSocketDisconnectedEvent += RefreshHomeMenuBindings;

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

        public string ProtectionStatusString => CreatePasswordProtectedDrawing ? "🔒" : "🔓";

        public string VisibilityStatusString => CreatePubliclyVisibleDrawing ? "Publique" : "Privée";

        public bool CreatePasswordProtectedDrawing
        {
            get => _createPasswordProtectedDrawing && IsOnline(this);
            private set => _createPasswordProtectedDrawing = value;
        }

        public bool CreatePubliclyVisibleDrawing
        {
            get => _createPubliclyVisibleDrawing && IsOnline(this);
            set => _createPubliclyVisibleDrawing = value;
        }

        public string LockColor => CreatePasswordProtectedDrawing ? "#FF2B3ACF" : "#FFB3B3B3";

        public string VisibilityColor => CreatePubliclyVisibleDrawing ? "#FF2B3ACF" : "#FFB3B3B3";

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
        public RelayCommand<object> ToggleNewDrawingProtection { get; set; }
        public RelayCommand<object> ToggleNewDrawingVisibility { get; set; }

        public void Dispose()
        {
            WebSocketConnectedEvent -= RefreshHomeMenuBindings;
        }

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

        private static void RefreshHomeMenuBindings(object sender, EventArgs a)
        {
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(CommandManager
                                                                                         .InvalidateRequerySuggested);
        }

        private static void RefreshHomeMenuBindings(object sender, int a)
        {
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(CommandManager
                                                                                         .InvalidateRequerySuggested);
        }

        private void ToggleProtection(object o)
        {
            CreatePasswordProtectedDrawing = !CreatePasswordProtectedDrawing;
            OnPropertyChanged("ProtectionStatusString");
            OnPropertyChanged("CreatePasswordProtectedDrawing");
            OnPropertyChanged("LockColor");
        }

        private void ToggleDrawingVisibility(object obj)
        {
            CreatePubliclyVisibleDrawing = !CreatePubliclyVisibleDrawing;
            OnPropertyChanged("VisibilityStatusString");
            OnPropertyChanged("CreatePubliclyVisibleDrawing");
            OnPropertyChanged("VisibilityColor");
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
                LoginWindow.Closing += (s, a) => LoginWindow = null;
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
                    UserAlerts.ShowErrorMessage("Une erreur est survenue lors de l'ouverture du fichier. Exception #" +
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
                case EditingModeOption.Pixel: break;
                default: return;
            }

            if (CreatePasswordProtectedDrawing)
            {
                if (obj is PasswordBox password && !string.IsNullOrWhiteSpace(password.Password))
                {
                    _homeMenu.CreateNewDrawing(NewDrawingName, selectedMode, password.Password,
                                               CreatePubliclyVisibleDrawing);
                }
                else
                {
                    UserAlerts.ShowErrorMessage("Le mot de passe est invalide");
                }
            }
            else
            {
                _homeMenu.CreateNewDrawing(NewDrawingName, selectedMode,
                                           visibilityPublic: CreatePubliclyVisibleDrawing);
            }
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

            bool drawingProtected = SelectedOnlineDrawing.Protection.Active &&
                                    Username != SelectedOnlineDrawing.Owner.Username;

            _homeMenu.JoinOnlineDrawing(SelectedOnlineDrawing.Id, drawingProtected);
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
            Tuple<string, string, EditingModeOption, List<StrokeModel>> drawingParams)
        {
            DrawingRoomId = drawingParams.Item1;
            DrawingName = drawingParams.Item2;
            OpenEditorWindow(drawingParams.Item3, drawingParams.Item4);
        }

        private static void OnlineDrawingLoadingFailedHandler(object sender, string e)
        {
            UserAlerts.ShowErrorMessage(e);
        }

        private void OpenEditorWindow(EditingModeOption option = EditingModeOption.Trait,
            List<StrokeModel> strokes = null)
        {
            if (option == EditingModeOption.Trait)
            {
                if (StrokeEditor == null)
                {
                    PixelEditor = null;
                    StrokeEditor = new StrokeEditorView();
                    StrokeEditor.Show();
                    if (strokes != null)
                    {
                        (StrokeEditor.DataContext as StrokeEditorViewModel)?.RebuildDrawing(strokes);
                    }

                    StrokeEditor.Closing += OnEditorClosedHandler;
                    OnClosingRequest();
                }
            }
            else if (option == EditingModeOption.Pixel)
            {
                if (PixelEditor == null)
                {
                    StrokeEditor = null;
                    PixelEditor = new PixelEditorView();
                    PixelEditor.Show();
                    // TODO: Modify this function once server saving protocol is established
                    //(PixelEditor.DataContext as StrokeEditorViewModel)?.ReplayActions(strokes);
                    PixelEditor.Closing += OnEditorClosedHandler;
                    OnClosingRequest();
                }
            }
        }

        private void OpenEditorWindow(StrokeCollection strokes)
        {
            if (StrokeEditor == null)
            {
                StrokeEditor = new StrokeEditorView();
                StrokeEditor.Show();
                if (StrokeEditor.DataContext is StrokeEditorViewModel editorStrokeViewModel)
                {
                    foreach (Stroke stroke in strokes)
                    {
                        editorStrokeViewModel.StrokesCollection.Add(stroke);
                    }
                }

                StrokeEditor.Closing += OnEditorClosedHandler;
                OnClosingRequest();
            }
        }

        private void OpenEditorWindow(string autosaveDrawingName)
        {
            if (StrokeEditor == null)
            {
                StrokeEditor = new StrokeEditorView();
                StrokeEditor.Show();
                if (StrokeEditor.DataContext is StrokeEditorViewModel editorStrokeViewModel)
                {
                    editorStrokeViewModel.LoadAutosaved.Execute(autosaveDrawingName);
                }

                StrokeEditor.Closing += OnEditorClosedHandler;
                OnClosingRequest();
            }
        }

        private void OnEditorClosedHandler(object sender, EventArgs e)
        {
            if (sender is StrokeEditorView strokeEditorView)
            {
                (strokeEditorView.DataContext as StrokeEditorViewModel)?.UnsubscribeDrawingRoom();
                (strokeEditorView.DataContext as StrokeEditorViewModel)?.Dispose();
            }
            else if (sender is PixelEditorView pixelEditorView)
            {
                (pixelEditorView.DataContext as PixelEditorlViewModel)?.Dispose();
            }

            if (HomeMenu == null)
            {
                HomeMenu = new HomeMenu();
                HomeMenu.Show();
                HomeMenu.Closing += (s, a) => HomeMenu = null;
                OnClosingRequest();
            }

            StrokeEditor = null;
            PixelEditor = null;
        }
    }
}
