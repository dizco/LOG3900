using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolyPaint.ViewModels;
using System.Diagnostics;

namespace PolyPaint.Views
{
    public partial class DrawingPixelWindowView : Window
    {
        private static WriteableBitmap writeableBmp;
        private Point _newPosition;
        private Point _oldPosition;
        private int _thickness = 10;

        public DrawingPixelWindowView()
        {
            InitializeComponent();
            DataContext = new DrawingPixelWindowViewModel();
            InitiateBitmap();
        }

        private void GlisserCommence(object sender, DragStartedEventArgs e)
        {
            (sender as Thumb).Background = Brushes.Black;
        }

        private void GlisserTermine(object sender, DragCompletedEventArgs e)
        {
            (sender as Thumb).Background = Brushes.White;
        }

        private void GlisserMouvementRecu(object sender, DragDeltaEventArgs e)
        {
            string nom = (sender as Thumb).Name;
            if (nom == "horizontal" || nom == "diagonal")
                colonne.Width = new GridLength(Math.Max(32, colonne.Width.Value + e.HorizontalChange));
            if (nom == "vertical" || nom == "diagonal")
                ligne.Height = new GridLength(Math.Max(32, ligne.Height.Value + e.VerticalChange));
        }

        private void SurfaceDessinMouseEnter(object sender, MouseEventArgs e)
        {
            _oldPosition = e.GetPosition(SurfaceDessin);
        }

        private void SurfaceDessinPreviewMouseDown(object sender, MouseEventArgs e)
        {
            _oldPosition = e.GetPosition(SurfaceDessin);
            //ToDo: SetPixel to color one point
        }

        private void InitiateBitmap()
        {
            // http://writeablebitmapex.codeplex.com/
            //Todo: set Bitmap size depending of the canvas stretched size
            //Todo: initiate in MVVM

            // Initialize the WriteableBitmap with two times the size of the window
            //and set it as source of an Image control
            writeableBmp = BitmapFactory.New((int)Width * 2, (int)Height * 2);
            using (writeableBmp.GetBitmapContext())
            {
                DrawnImage.Source = writeableBmp;
            }

            // Clear the WriteableBitmap with white color
            writeableBmp.Clear(Colors.Transparent);         
        }

        private void SurfaceDessinMouseMove(object sender, MouseEventArgs e)
        {
            //Todo: Combine tools in switch case
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _newPosition = e.GetPosition(SurfaceDessin);
                (DataContext as DrawingPixelWindowViewModel)?.DrawPixel(writeableBmp,_oldPosition, _newPosition);
                (DataContext as DrawingPixelWindowViewModel)?.ErasePixel(writeableBmp, _oldPosition, _newPosition);
                _oldPosition = _newPosition;
           
                
            }
        }
    }
}
