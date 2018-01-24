using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PolyPaint.Modeles
{
    class ChatWindowModele : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string pendingMessageText = "Entrez votre message ici";
        public string PublicMessageText
        {
            get { return pendingMessageText; }
            set { pendingMessageText = value; ProprieteModifiee(); }
        }

        protected void ProprieteModifiee([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


}



       