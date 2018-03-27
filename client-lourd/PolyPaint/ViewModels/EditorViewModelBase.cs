using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolyPaint.Constants;
using PolyPaint.Helpers;

namespace PolyPaint.ViewModels
{
    internal class EditorViewModelBase : ViewModelBase, IDisposable
    {
        private Canvas _currentCanvas;
        private InkCanvas _currentInkCanvas;

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

        private void OnEditorPollRequestReceived(object sender, EventArgs eventArgs)
        {
            MemoryStream imageStream = _currentCanvas == null
                                           ? BuildImageStream(_currentInkCanvas)
                                           : BuildImageStream(_currentCanvas);

            byte[] imageBytes = imageStream.ToArray();

            string base64Image = Convert.ToBase64String(imageBytes);
        }

        private MemoryStream BuildImageStream([Optional] object canvasObject, [Optional] object sender,
            [Optional] string imageFormat)
        {
            MemoryStream imageStream = new MemoryStream();

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
            else if (canvasObject == null)
            {
                // When neither a StrokeEditor, PixelEditor not EditorViewModelBase is requesting a memory stream of the current drawing
                return null;
            }

            int imageHeight = (int) ((canvasObject as Canvas)?.ActualHeight ??
                                     (canvasObject as InkCanvas)?.ActualHeight ?? 0);
            int imageWidth =
                (int) ((canvasObject as Canvas)?.ActualWidth ?? (canvasObject as InkCanvas)?.ActualWidth ?? 0);

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
                case ".jpg": //jpg
                    encoder = new JpegBitmapEncoder();
                    ((JpegBitmapEncoder) encoder).QualityLevel = 100;
                    break;
                default: //bmp
                    encoder = new BmpBitmapEncoder();
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
                fileImageStream?.Close();
            }
        }
    }
}
