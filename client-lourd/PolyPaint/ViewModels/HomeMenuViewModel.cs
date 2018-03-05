using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Views;

namespace PolyPaint.ViewModels
{
    internal class HomeMenuViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public HomeMenuViewModel()
        {
            NewDrawingCommand = new RelayCommand<object>(OpenNewDrawingSubMenu);
            OldDrawingCommand = new RelayCommand<object>(OpenOldDrawing);
            JoinDrawingCommand = new RelayCommand<object>(OpenOnlineDrawingSubMenu, IsOnline);
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
        public CollectionViewSource FilteredDrawings { get; set; }
        public List<OnlineDrawingModel> OnlineDrawingList { get; set; } = new List<OnlineDrawingModel>();

        public string DrawingSearchTerms { get; set; }

        public string DrawingName { get; set; }

        public RelayCommand<object> GalleryCommand { get; set; }
        public RelayCommand<object> JoinDrawingCommand { get; set; }
        public RelayCommand<object> NewDrawingCommand { get; set; }
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
            LoadDrawings();
            MainMenuVisibility = Visibility.Collapsed;
            NewDrawingVisibility = Visibility.Collapsed;
            JoinDrawingVisibility = Visibility.Visible;
            UpdateVisibilityProperties();
        }

        private async void LoadDrawings()
        {
            OnlineDrawingList.Clear();

            int currentPage = 1;
            int MaxPages = 0;
            int MaxRetries = 3;
            int retryCount = 0;
            do
            {
                HttpResponseMessage response = await RestHandler.AllDrawings(currentPage);
                if (!response.IsSuccessStatusCode)
                {
                    retryCount++;
                    continue;
                }

                try
                {
                    JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());
                    MaxPages = content.GetValue("pages").ToObject<int>();
                    OnlineDrawingModel[] docsArray = content.GetValue("docs").ToObject<OnlineDrawingModel[]>();
                    foreach (OnlineDrawingModel drawing in docsArray) OnlineDrawingList.Add(drawing);
                }
                catch
                {
                    retryCount++;
                    continue;
                }

                currentPage++;
                retryCount = 0;
            } while (currentPage < MaxPages && retryCount < MaxRetries);
        }

        private void UpdateVisibilityProperties()
        {
            DrawingName = string.Empty;
            OnPropertyChanged("MainMenuVisibility");
            OnPropertyChanged("NewDrawingVisibility");
            OnPropertyChanged("JoinDrawingVisibility");
        }

        internal class OnlineDrawingModel
        {
            [JsonProperty(PropertyName = "name")]
            private string Name { get; set; }

            [JsonProperty(PropertyName = "_id")]
            private string Id { get; set; }
        }
    }
}
