using System;
using System.Windows.Controls;
using PolyPaint.ViewModels.Gallery;

namespace PolyPaint.Views.Gallery
{
    /// <summary>
    ///     Interaction logic for GalleryItemView.xaml
    /// </summary>
    public partial class GalleryItemView : UserControl
    {
        public GalleryItemView()
        {
            InitializeComponent();
        }

        public GalleryItemView(string drawingName, string drawingId, bool isOwner, bool isLocked, bool isPublic) :
            this()
        {
            DataContext = new GalleryItemViewModel(drawingName, drawingId, isOwner, isLocked, isPublic);
            ((GalleryItemViewModel) DataContext).ClosingRequest +=
                (sender, args) => ClosingRequest?.Invoke(sender, args);
        }

        public event EventHandler ClosingRequest;
    }
}
