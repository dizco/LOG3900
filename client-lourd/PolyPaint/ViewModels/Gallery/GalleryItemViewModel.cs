using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using PolyPaint.Annotations;
using PolyPaint.Helpers.Communication;

namespace PolyPaint.ViewModels.Gallery
{
    internal class GalleryItemViewModel : INotifyPropertyChanged, IDisposable
    {
        private const int RefreshTimeoutSeconds = 10;

        private readonly bool _drawingIsLocked;

        private readonly EventHandler _loadDrawingThumbnailFinishedEvent;
        private readonly bool _isDrawingOwner;
        private string _drawingId;
        private string _drawingName;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;

        private Bitmap _image;

        public GalleryItemViewModel(string drawingName, string drawingId, bool isOwner, bool isLocked)
        {
            DrawingId = drawingId;
            DrawingName = drawingName;
            _isDrawingOwner = isOwner;
            _drawingIsLocked = isLocked;
            LoadDrawingThumbnail();

            _cancellationToken = _cancellationTokenSource.Token;

            void RefreshThumbnailAction()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Thread.Sleep(TimeSpan.FromSeconds(RefreshTimeoutSeconds));

                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                LoadDrawingThumbnail();
            }

            _loadDrawingThumbnailFinishedEvent += (s, e) =>
            {
                Task refreshThumnailTask = new Task((Action) RefreshThumbnailAction, _cancellationToken);
                refreshThumnailTask.Start();
            };
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

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
                if (Image != null)
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(Image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                                                                 BitmapSizeOptions.FromEmptyOptions());
                }

                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
