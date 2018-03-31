using System.Windows;
using PolyPaint.ViewModels.Gallery;

namespace PolyPaint.Views.Gallery
{
    /// <summary>
    ///     Interaction logic for GalleryWindowView.xaml
    /// </summary>
    public partial class GalleryWindowView : Window
    {
        public GalleryWindowView()
        {
            InitializeComponent();
            DataContext = new GalleryViewModel();
            ((GalleryViewModel) DataContext).ClosingRequest += (sender, args) => Close();
        }
    }
}
