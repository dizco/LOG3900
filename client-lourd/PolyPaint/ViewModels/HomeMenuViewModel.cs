using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PolyPaint.Helpers;

namespace PolyPaint.ViewModels
{
    internal class HomeMenuViewModel : ViewModelBase
    {
        public HomeMenuViewModel()
        {
            NewDrawingCommand = new RelayCommand<object>(OpenNewDrawing);
            OldDrawingCommand = new RelayCommand<object>(OpenOldDrawing);
            JoinDrawingCommand = new RelayCommand<object>(OpenOnlineDrawing, IsOnline);
            GalleryCommand = new RelayCommand<object>(OpenGallery);

            WebSocketConnectedEvent += (s, a) => RefreshHomeMenuBindings();
        }

        public RelayCommand<object> GalleryCommand { get; set; }
        public RelayCommand<object> JoinDrawingCommand { get; set; }
        public RelayCommand<object> NewDrawingCommand { get; set; }
        public RelayCommand<object> OldDrawingCommand { get; set; }

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

        private void OpenOnlineDrawing(object obj)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
