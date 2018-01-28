using PolyPaint.Utilitaires;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.VueModeles
{
    internal class ViewModelBase
    {
        private static Messenger _mesenger;

        protected Messenger Messenger
        {
            get { return _mesenger; }
        }

        protected static Messenger StartMessenger(string uri)
        {
            if (_mesenger == null)
            {
                _mesenger = new Messenger(new SocketHandler(uri));
            }
            return _mesenger;
        }
    }
}
