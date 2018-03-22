using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PolyPaint.Helpers;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Models.PixelModels;
using PolyPaint.Strategy.PixelEditorActionStrategy;
using PolyPaint.Views;

namespace PolyPaint.ViewModels
{
    internal class PixelEditorViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private readonly PixelEditor _pixelEditor = new PixelEditor();

        private Visibility _chatDocked = Visibility.Collapsed;

        public PixelEditorViewModel()
        {
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, DrawingPixelModelPropertyModified est appelée.
            _pixelEditor.DrawingName = DrawingName;
            _pixelEditor.DrewLineEvent += PixelEditorDrewLineEventHandler;

            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChooseTool = new RelayCommand<string>(_pixelEditor.SelectTool);

            ExportImageCommand = new RelayCommand<object>(_pixelEditor.ExportImagePrompt);

            OpenHistoryCommand = new RelayCommand<object>(OpenHistory);

            if (Messenger?.IsConnected ?? false)
            {
                ChatDocked = Visibility.Visible;
            }

            SubscribeDrawingRoom();

            LoginStatusChanged += ProcessLoginStatusChange;

            ChangeEditorChatDisplayState += ChatDisplayStateChanged;

            PixelEditorActionReceived += ProcessPixelEditorActionReceived;
        }

        private void ProcessPixelEditorActionReceived(object sender, PixelEditorActionModel action)
        {
            EditorActionStrategyContext context = new EditorActionStrategyContext(action);
            context.ExecuteStrategy(_pixelEditor);
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

        private void PixelEditorDrewLineEventHandler(object o, List<Tuple<Point, string>> pixels)
        {
            SendNewPixels(pixels);
        }

        public WriteableBitmap WriteableBitmap
        {
            get => _pixelEditor.WriteableBitmap;
            set => _pixelEditor.WriteableBitmap = value;
        }

        public string ToolSelected
        {
            get => _pixelEditor.SelectedTool;
            set => PropertyModified();
        }

        public string ColorSelected
        {
            get => _pixelEditor.SelectedColor;
            set => _pixelEditor.SelectedColor = value;
        }

        public int PixelSizeSelected
        {
            get => _pixelEditor.PixelSize;
            set => _pixelEditor.PixelSize = value;
        }

        public static HistoryWindowView HistoryWindow { get; set; }

        //Commands for choosing the tools
        public RelayCommand<string> ChooseTool { get; set; }

        public RelayCommand<object> ExportImageCommand { get; set; }

        //Command for managing the views
        public RelayCommand<object> OpenChatWindowCommand { get; set; }
        public RelayCommand<object> ShowChatWindowCommand { get; set; }

        public RelayCommand<object> OpenHistoryCommand { get; set; }

        public void Dispose()
        {
            LoginStatusChanged -= ProcessLoginStatusChange;
            ChangeEditorChatDisplayState -= ChatDisplayStateChanged;
            CloseHistory();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public void PixelDraw(Point oldPoint, Point newPoint)
        {
            _pixelEditor.DrawPixels(oldPoint, newPoint);
        }

        public void PixelCursors(Border displayArea)
        {
            _pixelEditor.PixelCursor(displayArea);
        }

        private void ProcessLoginStatusChange(object sender, string username)
        {
            _pixelEditor.CurrentUsername = username;
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

        public void OpenChatWindow(object o)
        {
            if (ChatWindow == null)
            {
                ChatWindow = new ChatWindowView();
                ChatWindow.Show();
                ChatWindow.Closing += (sender, args) => ChatWindow = null;
            }
            else
            {
                ChatWindow.Activate();
            }
        }

        internal void SendNewPixels(List<Tuple<Point, string>> pixels)
        {
            Messenger?.SendEditorActionNewPixels(pixels);
        }

        private void SubscribeDrawingRoom()
        {
            Messenger?.SubscribeToDrawing();
        }

        public void UnsubscribeDrawingRoom()
        {
            Messenger?.UnsubscribeToDrawing();
        }

        public void OpenHistory(object o)
        {
            if (HistoryWindow == null)
            {
                HistoryWindow = new HistoryWindowView();
                HistoryWindow.Show();
                HistoryWindow.Closed += (sender, args) => HistoryWindow = null;
            }
            else
            {
                HistoryWindow.Activate();
            }
        }

        public void CloseHistory()
        {
            HistoryWindow?.Close();
            HistoryWindow = null;
        }
    }
}
