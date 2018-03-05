using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using PolyPaint.Helpers;

namespace PolyPaint.ViewModels
{
    internal class HomeMenuViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public HomeMenuViewModel()
        {
            NewDrawingCommand = new RelayCommand<object>(OpenNewDrawing);
            OldDrawingCommand = new RelayCommand<object>(OpenOldDrawing);
            JoinDrawingCommand = new RelayCommand<object>(OpenOnlineDrawing, IsOnline);
            GalleryCommand = new RelayCommand<object>(OpenGallery);
            GoToMenuCommand = new RelayCommand<object>(OpenMenu);

            WebSocketConnectedEvent += (s, a) => RefreshHomeMenuBindings();
        }

        public Visibility MainMenuVisibility { get; set; } = Visibility.Visible;
        public Visibility NewDrawingVisibility { get; set; } = Visibility.Collapsed;
        public Visibility JoinDrawingVisibility { get; set; } = Visibility.Collapsed;
        public CollectionViewSource FilteredDrawings { get; set; }

        public string DrawingSearchTerms { get; set; }

        public string DrawingName { get; set; }

        public RelayCommand<object> GalleryCommand { get; set; }
        public RelayCommand<object> JoinDrawingCommand { get; set; }
        public RelayCommand<object> NewDrawingCommand { get; set; }
        public RelayCommand<object> OldDrawingCommand { get; set; }
        public RelayCommand<object> GoToMenuCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshHomeMenuBindings()
        {
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
            {
                CommandManager
                    .InvalidateRequerySuggested();
            });
        }

        private bool IsOnline(object obj)
        {
            return Messenger?.IsConnected ?? false;
        }

        private void OpenGallery(object obj)
        {
            throw new NotImplementedException();
        }

        private void OpenOldDrawing(object obj)
        {
            throw new NotImplementedException();
        }

        private void OpenNewDrawing(object obj)
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

        private void OpenOnlineDrawing(object obj)
        {
            MainMenuVisibility = Visibility.Collapsed;
            NewDrawingVisibility = Visibility.Collapsed;
            JoinDrawingVisibility = Visibility.Visible;
            UpdateVisibilityProperties();
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
