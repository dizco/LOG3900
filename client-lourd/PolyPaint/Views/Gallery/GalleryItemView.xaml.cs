using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PolyPaint.ViewModels.Gallery;

namespace PolyPaint.Views.Gallery
{
    /// <summary>
    /// Interaction logic for GalleryItemView.xaml
    /// </summary>
    public partial class GalleryItemView : UserControl
    {
        public GalleryItemView()
        {
            InitializeComponent();
        }

        public GalleryItemView(string drawingName, string drawingId, bool isOwner, bool isLocked):this()
        {
            DataContext = new GalleryItemViewModel(drawingName, drawingId, isOwner, isLocked);
        }
    }
}
