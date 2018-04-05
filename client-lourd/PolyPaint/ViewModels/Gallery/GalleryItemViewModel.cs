using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolyPaint.Annotations;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models;
using PolyPaint.Models.ApiModels;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Views;

namespace PolyPaint.ViewModels.Gallery
{
    internal class GalleryItemViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private const int RefreshTimeoutSeconds = 10;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly bool _isDrawingOwner;

        private readonly EventHandler _loadDrawingThumbnailFinishedEvent;

        private string _drawingId;

        private bool _drawingIsLocked;
        private bool _drawingIsPublic;
        private string _drawingName;

        private Bitmap _image;

        public GalleryItemViewModel(string drawingName, string drawingId, bool isOwner, bool isLocked, bool isPublic)
        {
            DrawingId = drawingId;
            DrawingName = drawingName;
            _isDrawingOwner = isOwner;
            _drawingIsLocked = isLocked;
            _drawingIsPublic = isPublic;

            TogglePasswordCommand = new RelayCommand<object>(TogglePasswordProtection);
            ToggleVisibilityCommand = new RelayCommand<object>(ToggleVisibility);
            JoinDrawingCommand = new RelayCommand<object>(o => JoinOnlineDrawing());

            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            void RefreshThumbnailAction()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Thread.Sleep(TimeSpan.FromSeconds(RefreshTimeoutSeconds));

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                LoadDrawingThumbnail();
            }

            _loadDrawingThumbnailFinishedEvent += (s, e) =>
            {
                Task refreshThumnailTask = new Task(RefreshThumbnailAction, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    refreshThumnailTask.Start();
                }
            };

            LoadDrawingThumbnail();
        }

        public RelayCommand<object> TogglePasswordCommand { get; set; }
        public RelayCommand<object> ToggleVisibilityCommand { get; set; }
        public RelayCommand<object> JoinDrawingCommand { get; set; }

        public string DrawingId
        {
            get => _drawingId;
            set
            {
                _drawingId = value;
                PropertyModified();
            }
        }

        public string DrawingName
        {
            get => _drawingName;
            set
            {
                _drawingName = value;
                PropertyModified();
            }
        }

        public string DrawingLockStatus => _drawingIsLocked ? "🔒" : "🔓";

        public string DrawingLockCursor => _isDrawingOwner ? "Hand" : "Cursor";

        public string DrawingVisibilityCursor => _isDrawingOwner ? "Hand" : "Cursor";

        public string DrawingVisibilityStatus => _drawingIsPublic ? "🐵" : "🙈";

        public string VisibilityToolTipText => _drawingIsPublic ? "Dessin public" : "Dessin privé";

        private Bitmap Image
        {
            get => _image;
            set
            {
                _image = value;
                PropertyModified(nameof(ImageSource));
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                ImageSource source = null;
                if (Image != null)
                {
                    IntPtr imageHBitmap = Image.GetHbitmap();
                    try
                    {
                        source = Imaging.CreateBitmapSourceFromHBitmap(imageHBitmap, IntPtr.Zero,
                                                                       Int32Rect.Empty,
                                                                       BitmapSizeOptions.FromEmptyOptions());
                    }
                    finally
                    {
                        DeleteObject(imageHBitmap);
                    }
                }

                return source ??
                       new BitmapImage(new Uri("/PolyPaint;component/Resources/Misc/empty_drawing_placeholder.jpg",
                                               UriKind.RelativeOrAbsolute));
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ClosingRequest;

        private async void LoadDrawingThumbnail()
        {
            HttpResponseMessage response = await RestHandler.GetDrawingThumbnail(DrawingId);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string base64Image = JObject.Parse(await response.Content.ReadAsStringAsync())["thumbnail"]
                                                .ToString();
                    ConvertBase64ToImage(base64Image);
                }
                catch
                {
                    // ignored
                }
            }

            _loadDrawingThumbnailFinishedEvent?.Invoke(this, null);
        }

        /// <summary>
        ///     Raises ClosingRequest to trigger the closing of the LoginWindowView
        /// </summary>
        private void OnClosingRequest()
        {
            ClosingRequest?.Invoke(this, EventArgs.Empty);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ConvertBase64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);

            MemoryStream imageStream = new MemoryStream(imageBytes, 0, imageBytes.Length);

            Bitmap bitmapImage = new Bitmap(imageStream);

            imageStream.Close();
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() => { Image = bitmapImage; });
        }

        private async void TogglePasswordProtection(object obj)
        {
            if (_drawingIsLocked && _isDrawingOwner)
            {
                HttpResponseMessage response = await RestHandler.UpdateDrawingProtection(_drawingId);
                if (response.IsSuccessStatusCode)
                {
                    _drawingIsLocked = false;
                }
            }
            else if (_isDrawingOwner)
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

            PropertyModified(nameof(DrawingLockStatus));
        }

        private async void ToggleVisibility(object obj)
        {
            if (_isDrawingOwner)
            {
                HttpResponseMessage response = await RestHandler.UpdateDrawingVisibility(_drawingId, !_drawingIsPublic);
                if (response.IsSuccessStatusCode)
                {
                    _drawingIsPublic = !_drawingIsPublic;
                }
            }

            PropertyModified(nameof(VisibilityToolTipText));
            PropertyModified(nameof(DrawingVisibilityStatus));
        }

        internal async void JoinOnlineDrawing()
        {
            HttpResponseMessage response;

            if (!_isDrawingOwner && _drawingIsLocked)
            {
                string drawingPassword = null;

                PasswordPrompt passwordPrompt = new PasswordPrompt();

                passwordPrompt.PasswordEntered += (s, password) =>
                {
                    drawingPassword = password;
                    passwordPrompt.Close();
                };

                passwordPrompt.ShowDialog();

                if (drawingPassword != null)
                {
                    response = await RestHandler.GetOnlineDrawing(_drawingId, drawingPassword);
                }
                else
                {
                    return;
                }
            }
            else
            {
                response = await RestHandler.GetOnlineDrawing(_drawingId);
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    JObject content = JObject.Parse(await response.Content.ReadAsStringAsync());

                    Dictionary<string, int> users = content.GetValue("users").ToObject<Dictionary<string, int>>();
                    string editingMode = content.GetValue("mode").ToString();

                    users.TryGetValue("active", out int activeUsers);
                    users.TryGetValue("limit", out int limitUsers);
                    if (activeUsers >= limitUsers)
                    {
                        OnOnlineDrawingJoinFailed($"Impossible d\'ouvrir le dessin. Le nombre maximal d\'éditeurs a été atteint.");
                        return;
                    }

                    List<StrokeModel> strokes = content.GetValue("strokes").ToObject<List<StrokeModel>>();
                    string drawingName = content.GetValue("name").ToString();

                    EditingModeOption option = EditingModeOption.Trait;
                    if (editingMode == "stroke")
                    {
                        option = EditingModeOption.Trait;
                    }
                    else if (editingMode == "pixel")

                    {
                        option = EditingModeOption.Pixel;
                    }

                    OnOnlineDrawingJoined(_drawingId, drawingName, option, strokes);
                }
                catch
                {
                    UserAlerts.ShowErrorMessage("Une erreur est survenue");
                }
            }
            else
            {
                HomeMenuModel.OnResponseError(await response?.Content.ReadAsStringAsync());
            }
        }

        private void OnOnlineDrawingJoined(string drawingId, string drawingName, EditingModeOption option,
            List<StrokeModel> strokes)
        {
            DrawingRoomId = drawingId;
            ViewModelBase.DrawingName = drawingName;
            IsDrawingOwner = _isDrawingOwner;
            IsPasswordProtected = _drawingIsLocked;
            IsPubliclyVisible = _drawingIsPublic;

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

        private void OnOnlineDrawingJoinFailed(string error)
        {
            UserAlerts.ShowErrorMessage(error);
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
