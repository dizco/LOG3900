using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;
using PolyPaint.Annotations;
using PolyPaint.Constants;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Views;
using Application = System.Windows.Application;

namespace PolyPaint.ViewModels
{
    internal class EditorViewModelBase : ViewModelBase, IDisposable, INotifyPropertyChanged
    {
        private const int WaitTimeForThread = 5;
        private const int ImageHeight = 720;
        private const int ImageWidth = 1280;
        private Canvas _currentCanvas;
        private InkCanvas _currentInkCanvas;

        protected EditorViewModelBase()
        {
            EditorPollRequestReceived += OnEditorPollRequestReceived;

            ToggleVisibilityCommand = new RelayCommand<object>(ToggleVisibility);
            TogglePasswordCommand = new RelayCommand<object>(TogglePasswordProtection);
            PublishAsTemplateCommand = new RelayCommand<object>(PublishAsTemplate);
            OpenHistoryCommand = new RelayCommand<object>(OpenHistory);
            OpenTutorialCommand = new RelayCommand<string>(OpenTutorialWindow);
        }

        public static HistoryWindowView HistoryWindow { get; set; }

        protected object Canvas
        {
            set
            {
                switch (value)
                {
                    case Canvas canvas:
                        _currentCanvas = canvas;
                        _currentInkCanvas = null;
                        break;
                    case InkCanvas inkCanvas:
                        _currentCanvas = null;
                        _currentInkCanvas = inkCanvas;
                        break;
                }
            }
        }

        public RelayCommand<object> ToggleVisibilityCommand { get; set; }
        public RelayCommand<object> TogglePasswordCommand { get; set; }
        public RelayCommand<object> PublishAsTemplateCommand { get; set; }
        public RelayCommand<object> OpenHistoryCommand { get; set; }
        public RelayCommand<string> OpenTutorialCommand { get; set; }

        public string LockUnlockDrawingMessage => IsPasswordProtected
                                                      ? "Retirer la protection du dessin"
                                                      : "Protéger le dessin par un mot de passe";

        public bool AccessibilityToggleIsEnabled =>
            IsDrawingOwner && (Messenger?.IsConnected ?? false) && DrawingRoomId != null;

        public string DrawingVisibilityMessage => IsPubliclyVisible
                                                      ? "Retirer l'accès public de la galerie"
                                                      : "Rendre l'accès public de la galerie";

        public bool IsConnectedToDrawing => (Messenger?.IsConnected ?? false) && DrawingRoomId != null;

        public void Dispose()
        {
            EditorPollRequestReceived -= OnEditorPollRequestReceived;
            HistoryWindow?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void PublishAsTemplate(object obj)
        {
            HttpResponseMessage response = await RestHandler.PublishDrawingAsTemplate(DrawingRoomId);

            if (response.IsSuccessStatusCode)
            {
                UserAlerts.ShowInfoMessage("Le dessin actuel a été publié comme modèle");
            }
            else
            {
                string hintMessages;

                try
                {
                    hintMessages = await ServerErrorParser.ParseHints(response);
                }
                catch (JsonReaderException)
                {
                    hintMessages = "Un message d\'erreur d\'un format inconnu a été reçu et n\'a pu être traité";
                }

                UserAlerts.ShowErrorMessage(hintMessages);
            }
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

        public void OpenTutorial(int nSessions, string tutorialMode)
        {
            if (nSessions == 0)
            {
                OpenTutorialWindow(tutorialMode);
            }
        }

        public void OpenTutorialWindow(string tutorialMode)
        {
            TutorialWindowView tutorialWindow = new TutorialWindowView(tutorialMode);
            tutorialWindow.ShowDialog();
        }

        private async void ToggleVisibility(object obj)
        {
            HttpResponseMessage response = await RestHandler.UpdateDrawingVisibility(DrawingRoomId, !IsPubliclyVisible);
            if (response.IsSuccessStatusCode)
            {
                IsPubliclyVisible = !IsPubliclyVisible;
            }

            PropertyModified(nameof(DrawingVisibilityMessage));
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

                        passwordPrompt.Close();
                    }
                    else
                    {
                        string hintMessages;

                        try
                        {
                            hintMessages = await ServerErrorParser.ParseHints(response);
                        }
                        catch (JsonReaderException)
                        {
                            hintMessages =
                                "Un message d\'erreur d\'un format inconnu a été reçu et n\'a pu être traité";
                        }

                        UserAlerts.ShowErrorMessage(hintMessages);
                    }
                };

                passwordPrompt.ShowDialog();
            }

            PropertyModified(nameof(LockUnlockDrawingMessage));
            PropertyModified(nameof(AccessibilityToggleIsEnabled));
        }

        private async void OnEditorPollRequestReceived(object sender, EventArgs eventArgs)
        {
            try
            {
                AutoResetEvent copiedCanvas = new AutoResetEvent(false);
                MemoryStream imageStream = null;

                (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
                {
                    imageStream = BuildImageStream();
                    copiedCanvas.Set();
                });

                copiedCanvas.WaitOne(TimeSpan.FromSeconds(WaitTimeForThread));

                byte[] imageBytes = imageStream.ToArray();

                imageStream.Close();

                string base64Image = Convert.ToBase64String(imageBytes);

                HttpResponseMessage response = await RestHandler.UpdateDrawingThumbnail(DrawingRoomId, base64Image);
            }
            catch (AbandonedMutexException)
            {
                // ignored
                // Application thread couldn't copy the canvas in time
            }
        }

        private MemoryStream BuildImageStream([Optional] object sender,
            [Optional] string imageFormat)
        {
            MemoryStream imageStream = new MemoryStream();
            object canvasObject = null;

            if (sender is StrokeEditorViewModel)
            {
                // A StrokeEditor is requesting a memory stream of the current drawing
                canvasObject = _currentInkCanvas;
            }
            else if (sender is PixelEditorViewModel)
            {
                // A PixelEditor is requesting a memory stream of the current drawing
                canvasObject = _currentCanvas;
            }
            else if (_currentCanvas != null)
            {
                canvasObject = _currentCanvas;
            }
            else if (_currentInkCanvas != null)
            {
                canvasObject = _currentInkCanvas;
            }

            int imageHeight = (int) ((canvasObject as Canvas)?.ActualHeight ??
                                     (canvasObject as InkCanvas)?.ActualHeight ?? ImageHeight);
            int imageWidth = (int) ((canvasObject as Canvas)?.ActualWidth ??
                                    (canvasObject as InkCanvas)?.ActualWidth ?? ImageWidth);

            RenderTargetBitmap imageRender = new RenderTargetBitmap(imageWidth, imageHeight,
                                                                    ImageManipulationConstants.DotsPerInch,
                                                                    ImageManipulationConstants.DotsPerInch,
                                                                    PixelFormats.Pbgra32);

            switch (canvasObject)
            {
                case Canvas canvas:
                    imageRender.Render(canvas);
                    break;
                case InkCanvas inkCanvas:
                    imageRender.Render(inkCanvas);
                    break;
            }

            BitmapEncoder encoder;

            switch (imageFormat)
            {
                case ".png": //png
                    encoder = new PngBitmapEncoder();
                    break;
                case ".bmp": //bmp
                    encoder = new BmpBitmapEncoder();
                    break;
                default: //jpg
                    encoder = new JpegBitmapEncoder();
                    ((JpegBitmapEncoder) encoder).QualityLevel = 75;
                    break;
            }

            encoder.Frames.Add(BitmapFrame.Create(imageRender));

            encoder.Save(imageStream);

            return imageStream;
        }

        protected void ExportImagePrompt(object sender)
        {
            SaveFileDialog exportImageDialog = new SaveFileDialog
            {
                Title = "Exporter le dessin",
                Filter = FileExtensionConstants.ExportImageFilter,
                AddExtension = true
            };

            if (exportImageDialog.ShowDialog() != DialogResult.OK)
            {
                // User closed the dialog
                return;
            }

            string filePathNameExt = Path.GetFullPath(exportImageDialog.FileName);
            MemoryStream imageStream = BuildImageStream(this, Path.GetExtension(filePathNameExt));

            FileStream fileImageStream = null;
            try
            {
                fileImageStream = new FileStream(filePathNameExt, FileMode.Create);
                imageStream.WriteTo(fileImageStream);
            }
            catch (UnauthorizedAccessException)
            {
                UserAlerts.ShowErrorMessage("L'accès à ce fichier est interdit");
            }
            catch (Exception e)
            {
                UserAlerts
                    .ShowErrorMessage($"Une erreur est survenue.\n{e.Message}\nCode:{e.HResult & ((1 << 16) - 1)}");
            }
            finally
            {
                imageStream?.Close();
                fileImageStream?.Close();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
