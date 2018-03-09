
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PolyPaint.ViewModels
{
    internal class DrawingPixelWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
