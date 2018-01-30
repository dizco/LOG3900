﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PolyPaint.Modeles
{
    internal class ChatWindowModel : INotifyPropertyChanged
    {
        private string _pendingChatMessage = "Entrez votre message ici";

        public string PublicMessageText
        {
            get => _pendingChatMessage;
            set
            {
                _pendingChatMessage = value;
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