using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace PolyPaint
{
    class Document : Window
    {
        public InkCanvas InkCanvas;
        public Document(InkCanvas c)
        {
            InkCanvas = c;
        }

        public void SaveFile(string path)
        {
            Rect inkCanvasBounds = VisualTreeHelper.GetDescendantBounds(InkCanvas);
            double dpi = 96d;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)inkCanvasBounds.Width, (int)inkCanvasBounds.Height, dpi, dpi, PixelFormats.Default);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(InkCanvas);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), inkCanvasBounds.Size));
            }
            renderTargetBitmap.Render(drawingVisual);
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            try
            {
                MemoryStream saveStream = new MemoryStream();

                pngEncoder.Save(saveStream);

                saveStream.Close();
                saveStream.Dispose();
                System.Windows.Forms.SaveFileDialog exportImageDialog = new System.Windows.Forms.SaveFileDialog
                {
                    Title = "Save as",
                    Filter = "Bitmap files (*.bmp)|*.bmp|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|" +
                             "All files (*.*)|*.*"
                };
                if (path == null)
                {
                    if (exportImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string imageName = exportImageDialog.FileName;
                        File.WriteAllBytes(imageName, saveStream.ToArray());
                    }
                }
                else
                {
                    File.WriteAllBytes(path, saveStream.ToArray());
                }

            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
