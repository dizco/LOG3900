﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolyPaint.Helpers;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Models.PixelModels;
using PolyPaint.Strategy.PixelEditorActionStrategy;
using PolyPaint.Views;

namespace PolyPaint.ViewModels
{
    internal class PixelEditorViewModel : EditorViewModelBase
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

        //Commands for choosing the tools
        public RelayCommand<string> ChooseTool { get; set; }

        public RelayCommand<Canvas> ExportImageCommand { get; set; }

        //Command for managing the views
        public RelayCommand<object> OpenChatWindowCommand { get; set; }
        public RelayCommand<object> ShowChatWindowCommand { get; set; }

        //Pixel Rotate tool
        public RelayCommand<object> QuarterTurnClockwiseCommand { get; set; }
        public RelayCommand<object> QuarterTurnCounterClockwiseCommand { get; set; }
        public RelayCommand<object> VerticalFlipCommand { get; set; }
        public RelayCommand<object> HorizontalFlipCommand { get; set; }

        //Filters
        public RelayCommand<object> GrayFilterCommand { get; set; }
        public RelayCommand<object> InvertFilterCommand { get; set; }
        public RelayCommand<object> GaussianBlurFilterCommand { get; set; }

        public new void Dispose()
        {
            base.Dispose();
            LoginStatusChanged -= ProcessLoginStatusChange;
            ChangeEditorChatDisplayState -= ChatDisplayStateChanged;
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

        public void Fill(Point startPoint, double maxWidth, double maxHeight)
        {
            _pixelEditor.FloodFill(startPoint, maxWidth, maxHeight);
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

        public void ExportImagePrompt(InkCanvas drawingSurface)
        {
            // TODO: Validate exportation of empty drawing

            ExportImagePrompt(this);
        }

        /// <summary>
        ///     Loads all pixels from the server
        /// </summary>
        /// <param name="pixels">List of pixels to rebuild</param>
        internal void RebuildDrawing(List<PixelModel> pixels)
        {
            _pixelEditor.WriteableBitmap.Lock();

            foreach (PixelModel pixel in pixels)
            {
                _pixelEditor.WriteableBitmap.SetPixel((int) pixel.X, (int) pixel.Y,
                                                      (Color) ColorConverter.ConvertFromString(pixel.Color));
            }

            _pixelEditor.WriteableBitmap.Unlock();
        }
    }
}
