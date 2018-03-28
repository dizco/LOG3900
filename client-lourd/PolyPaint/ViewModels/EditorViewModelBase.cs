using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PolyPaint.Constants;
using PolyPaint.CustomComponents;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using Application = System.Windows.Application;

namespace PolyPaint.ViewModels
{
    internal class EditorViewModelBase : ViewModelBase, IDisposable
    {
        private Canvas _currentCanvas;
        private InkCanvas _currentInkCanvas;

        private const int WaitTimeForThread = 5;
        private const int ImageHeight = 720;
        private const int ImageWidth = 1280;

        protected EditorViewModelBase()
        {
            EditorPollRequestReceived += OnEditorPollRequestReceived;
        }

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

        public void Dispose()
        {
            EditorPollRequestReceived -= OnEditorPollRequestReceived;
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

            int imageHeight;
            int imageWidth;

            if (sender is StrokeEditorViewModel || sender is PixelEditorViewModel)
            {
                imageHeight = (int) ((canvasObject as Canvas)?.ActualHeight ??
                                     (canvasObject as InkCanvas)?.ActualHeight ?? 0);
                imageWidth =
                    (int) ((canvasObject as Canvas)?.ActualWidth ?? (canvasObject as InkCanvas)?.ActualWidth ?? 0);
            }
            else
            {
                imageHeight = ImageHeight;
                imageWidth = ImageWidth;
            }

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
            MemoryStream imageStream = BuildImageStream(sender: this, imageFormat: Path.GetExtension(filePathNameExt));

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
    }
}
