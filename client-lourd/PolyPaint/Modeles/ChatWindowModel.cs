using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PolyPaint.Modeles
{
    internal class ChatWindowModel : INotifyPropertyChanged
    {
        private string _pendingMessageText = "Entrez votre message ici";

        public string PublicMessageText
        {
            get => _pendingMessageText;
            set
            {
                _pendingMessageText = value;
                PropertyModified();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}