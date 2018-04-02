using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Models.PixelModels;
using PolyPaint.Strategy.PixelEditorActionStrategy;
using PolyPaint.Views;

namespace PolyPaint.ViewModels
{
    internal class PixelEditorViewModel : EditorViewModelBase, INotifyPropertyChanged
    {
        private readonly PixelEditor _pixelEditor = new PixelEditor();

        private Visibility _chatDocked = Visibility.Collapsed;

        public PixelEditorViewModel()
        {
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, DrawingPixelModelPropertyModified est appelée.
            _pixelEditor.DrawingName = DrawingName;
            _pixelEditor.DrewLineEvent += PixelEditorDrewLineEventHandler;
            _pixelEditor.PropertyChanged += (s, a) => PropertyModified(a.PropertyName);

            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChooseTool = new RelayCommand<string>(_pixelEditor.SelectTool);

            ExportImageCommand = new RelayCommand<Canvas>(ExportImagePrompt);

            OpenHistoryCommand = new RelayCommand<object>(OpenHistory);
            TogglePasswordCommand = new RelayCommand<object>(TogglePasswordProtection);

            //Pixel Rotate tool
            QuarterTurnClockwiseCommand = new RelayCommand<object>(_pixelEditor.QuarterTurnClockwise);
            QuarterTurnCounterClockwiseCommand = new RelayCommand<object>(_pixelEditor.QuarterTurnCounterClockwise);
            VerticalFlipCommand = new RelayCommand<object>(_pixelEditor.VerticalFlip);
            HorizontalFlipCommand = new RelayCommand<object>(_pixelEditor.HorizontalFlip);

            //Filters
            GrayFilterCommand = new RelayCommand<object>(_pixelEditor.GrayFilter);
            InvertFilterCommand = new RelayCommand<object>(_pixelEditor.InvertFilter);
            GaussianBlurFilterCommand = new RelayCommand<object>(_pixelEditor.GaussianBlurFilter);

            if (IsConnectedToDrawing)
            {
                ChatDocked = Visibility.Visible;
            }

            SubscribeDrawingRoom();

            LoginStatusChanged += ProcessLoginStatusChange;

            ChangeEditorChatDisplayState += ChatDisplayStateChanged;

            PixelEditorActionReceived += ProcessPixelEditorActionReceived;
        }

        public PixelEditorViewModel(Canvas canvas) : this()
        {
            Canvas = canvas;
        }

        public bool IsConnectedToDrawing => (Messenger?.IsConnected ?? false) && DrawingRoomId != null;

        public Visibility ChatDocked
        {
            get => _chatDocked;
            private set
            {
                _chatDocked = value;
                PropertyModified();
            }
        }

        public WriteableBitmap WriteableBitmap
        {
            get => _pixelEditor.WriteableBitmap;
            set => PropertyModified();
        }

        public WriteableBitmap CropWriteableBitmap
        {
            get => _pixelEditor.CropWriteableBitmap;
            set => PropertyModified();
        }

        public Point CropWriteableBitmapPosition
        {
            get => _pixelEditor.CropWriteableBitmapPosition;
            set => PropertyModified();
        }

        public bool IsWriteableBitmapOnEdition
        {
            get => _pixelEditor.IsWriteableBitmapOnEdition;
            set => PropertyModified();
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

        public RelayCommand<Canvas> ExportImageCommand { get; set; }

        //Command for managing the views
        public RelayCommand<object> OpenChatWindowCommand { get; set; }
        public RelayCommand<object> ShowChatWindowCommand { get; set; }

        public RelayCommand<object> OpenHistoryCommand { get; set; }
        public RelayCommand<object> TogglePasswordCommand { get; set; }

        //Pixel Rotate tool
        public RelayCommand<object> QuarterTurnClockwiseCommand { get; set; }
        public RelayCommand<object> QuarterTurnCounterClockwiseCommand { get; set; }
        public RelayCommand<object> VerticalFlipCommand { get; set; }
        public RelayCommand<object> HorizontalFlipCommand { get; set; }

        //Filters
        public RelayCommand<object> GrayFilterCommand { get; set; }
        public RelayCommand<object> InvertFilterCommand { get; set; }
        public RelayCommand<object> GaussianBlurFilterCommand { get; set; }

        public string LockUnlockDrawingMessage => IsPasswordProtected
                                                      ? "Retirer la protection du dessin"
                                                      : "Protéger le dessin par un mot de passe";

        public string LockUnlockIcon => IsPasswordProtected ? "🔒" : "🔓";

        public bool ProtectionToggleIsEnabled =>
            IsDrawingOwner && (Messenger?.IsConnected ?? false) && DrawingRoomId != null;

        public event PropertyChangedEventHandler PropertyChanged;

        public new void Dispose()
        {
            base.Dispose();
            LoginStatusChanged -= ProcessLoginStatusChange;
            ChangeEditorChatDisplayState -= ChatDisplayStateChanged;
            HistoryWindow?.Close();
        }

        private async void TogglePasswordProtection(object obj)
        {
            if (IsPasswordProtected)
            {
                HttpResponseMessage response = await RestHandler.UpdateDrawingProtection(DrawingRoomId);
                if (response.IsSuccessStatusCode)
                {
                    IsPasswordProtected = false;
                }
            }
            else
            {
                PasswordPrompt passwordPrompt = new PasswordPrompt();

                passwordPrompt.PasswordEntered += async (sender, password) =>
                {
                    HttpResponseMessage response = await RestHandler.UpdateDrawingProtection(DrawingRoomId, password);
                    if (response.IsSuccessStatusCode)
                    {
                        IsPasswordProtected = true;
                    }

                    passwordPrompt.Close();
                };

                passwordPrompt.ShowDialog();
            }

            PropertyModified("LockUnlockDrawingMessage");
            PropertyModified("ProtectionToggleIsEnabled");
            PropertyModified("LockUnlockIcon");
        }

        private void ProcessPixelEditorActionReceived(object sender, PixelEditorActionModel action)
        {
            EditorActionStrategyContext context = new EditorActionStrategyContext(action);
            context.ExecuteStrategy(_pixelEditor);
        }

        private void PixelEditorDrewLineEventHandler(object o, List<Tuple<Point, string>> pixels)
        {
            SendNewPixels(pixels);
        }

        public void ChangeCropWriteableBitmapPosition(Point position)
        {
            _pixelEditor.ChangeCropWriteableBitmapPosition(position);
        }

        public void PixelDraw(Point oldPoint, Point newPoint)
        {
            _pixelEditor.DrawPixels(oldPoint, newPoint);
        }

        public void PixelCursors(Border displayArea)
        {
            _pixelEditor.PixelCursor(displayArea);
        }

        public void ZoneSelector(Thumb selectedZoneThumb, Point oldPoint, Point newPoint)
        {
            _pixelEditor.SelectZone(selectedZoneThumb, oldPoint, newPoint);
        }

        public void BlitZoneSelector()
        {
            _pixelEditor.BlitSelectedZone();
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
                HistoryWindow.Closing += (sender, args) => HistoryWindow = null;
            }
            else
            {
                HistoryWindow.Activate();
            }
        }

        public void ExportImagePrompt(InkCanvas drawingSurface)
        {
            // TODO: Validate exportation of empty drawing

            ExportImagePrompt(this);
        }
    }
}
