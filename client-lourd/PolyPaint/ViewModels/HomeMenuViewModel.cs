using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PolyPaint.Helpers;
using PolyPaint.Models;
using PolyPaint.Models.ApiModels;
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
            FilteredDrawings = _homeMenu.FilteredDrawings;

            GoToNewDrawingSubMenuCommand = new RelayCommand<object>(OpenNewDrawingSubMenu);
            OldDrawingCommand = new RelayCommand<object>(OpenOldDrawing);
            GoToOnlineDrawingSubMenuCommand = new RelayCommand<object>(OpenOnlineDrawingSubMenu, IsOnline);
            JoinDrawingCommand = new RelayCommand<object>(JoinOnlineDrawing);
            GalleryCommand = new RelayCommand<object>(OpenGallery);
            GoToMenuCommand = new RelayCommand<object>(OpenMenu);
            BackToLogin = new RelayCommand<object>(OpenLogin);
            WebSocketConnectedEvent += (s, a) => RefreshHomeMenuBindings();
            if (string.IsNullOrEmpty(Username)) LoginButtonVisibility = Visibility.Visible;
        }

        public Visibility MainMenuVisibility { get; set; } = Visibility.Visible;
        public Visibility NewDrawingVisibility { get; set; } = Visibility.Collapsed;
        public Visibility JoinDrawingVisibility { get; set; } = Visibility.Collapsed;
        public Visibility LoginButtonVisibility { get; set; } = Visibility.Hidden;

        public ObservableCollection<OnlineDrawingModel> FilteredDrawings { get; set; }

        public string DrawingSearchTerms
        {
            set => _homeMenu.SearchTextChangedHandlers(value.ToLower());
        }

        public string DrawingName { get; set; }

        public OnlineDrawingModel SelectedOnlineDrawing { get; set; }

        public RelayCommand<object> GalleryCommand { get; set; }
        public RelayCommand<object> GoToOnlineDrawingSubMenuCommand { get; set; }
        public RelayCommand<object> JoinDrawingCommand { get; set; }
        public RelayCommand<object> GoToNewDrawingSubMenuCommand { get; set; }
        public RelayCommand<object> OldDrawingCommand { get; set; }
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

        private void RefreshHomeMenuBindings()
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
            throw new NotImplementedException();
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

        private void OpenOldDrawing(object obj)
        {
            throw new NotImplementedException();
        }

        private void OpenNewDrawingSubMenu(object obj)
        {
            MainMenuVisibility = Visibility.Collapsed;
            NewDrawingVisibility = Visibility.Visible;
            JoinDrawingVisibility = Visibility.Collapsed;
            UpdateVisibilityProperties();
        }

        private void OpenMenu(object obj)
        {
            MainMenuVisibility = Visibility.Visible;
            NewDrawingVisibility = Visibility.Collapsed;
            JoinDrawingVisibility = Visibility.Collapsed;
            UpdateVisibilityProperties();
        }

        private void OpenOnlineDrawingSubMenu(object obj)
        {
            _homeMenu.LoadDrawings();
            MainMenuVisibility = Visibility.Collapsed;
            NewDrawingVisibility = Visibility.Collapsed;
            JoinDrawingVisibility = Visibility.Visible;
            UpdateVisibilityProperties();
        }

        private void JoinOnlineDrawing(object obj)
        {
            if (SelectedOnlineDrawing == null) UserAlerts.ShowErrorMessage("Veuillez choisir un dessin");

            // TODO: Fetch drawing from server, open editor and load the drawing
        }

        private void UpdateVisibilityProperties()
        {
            DrawingName = string.Empty;
            OnPropertyChanged("MainMenuVisibility");
            OnPropertyChanged("NewDrawingVisibility");
            OnPropertyChanged("JoinDrawingVisibility");
        }
    }
}
