using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    public partial class DrawingPixelWindowView : Window
    {
        private static WriteableBitmap writeableBmp;
        private Point _newPosition;
        private Point _oldPosition;
        private readonly Image waveform = new Image();
        private int _thickness = 10;

        public DrawingPixelWindowView()
        {
            InitializeComponent();
            DataContext = new DrawingPixelWindowViewModel();
            InitiateBitmap();
            SurfaceDessin.MouseMove += myCanvas_MouseMove;
            SurfaceDessin.MouseEnter += SurfaceDessin_MouseEnterMouseDown;
            SurfaceDessin.PreviewMouseDown += SurfaceDessin_PreviewMouseDown;
        }

        private void SurfaceDessin_MouseEnterMouseDown(object sender, MouseEventArgs e)
        {
            _oldPosition = e.GetPosition(SurfaceDessin);
        }

        private void SurfaceDessin_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            _oldPosition = e.GetPosition(SurfaceDessin);
        }

        private void InitiateBitmap()
        {
            // http://writeablebitmapex.codeplex.com/
            writeableBmp = BitmapFactory.New(240, 1270);
            writeableBmp.Clear(Colors.White);

            using (writeableBmp.GetBitmapContext())
            {
                waveform.Source = writeableBmp;
                SurfaceDessin.Children.Add(waveform);
            }
        }

        private void myCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _newPosition = e.GetPosition(SurfaceDessin);
                DrawPixel(_oldPosition, _newPosition);
                _oldPosition = _newPosition;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                _newPosition = e.GetPosition(SurfaceDessin);
                ErasePixel(_oldPosition, _newPosition);
                _oldPosition = _newPosition;
            }
        }

        private void DrawPixel(Point oldPosition, Point newPosition)
        {
            //Todo: Add color change
            //Todo: Add Thickness change
            //Todo: Add pencil cursor

            for (int a = 0; a < _thickness; a++)
            {
                for (int i = 0; i < _thickness; i++)
                {
                    writeableBmp.DrawLine((int)oldPosition.X + i, (int)oldPosition.Y + a, (int)newPosition.X + i, (int)newPosition.Y + a, Colors.Black);
                }
            }

            // waveform.Source = writeableBmp;
        }

        private void ErasePixel(Point oldPosition, Point newPosition)
        {
            //Todo: Add Thickness change
            //Todo: Add Erasor cursor
            for (int a = 0; a < _thickness; a++)
            {
                for (int i = 0; i < _thickness; i++)
                {
                    writeableBmp.DrawLine((int) oldPosition.X + i, (int) oldPosition.Y + a, (int) newPosition.X + i,
                                          (int) newPosition.Y + a, Colors.White);
                }
            }
        }
    }
}
