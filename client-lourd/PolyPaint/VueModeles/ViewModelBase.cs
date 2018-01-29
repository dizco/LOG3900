using System;
using Newtonsoft.Json.Linq;
using PolyPaint.Utilitaires;

namespace PolyPaint.VueModeles
{
    internal class ViewModelBase
    {
        private static Messenger _mesenger;

        protected Messenger Messenger => _mesenger;

        protected static event EventHandler<JObject> ChatMessageReceived;
        protected static event EventHandler<JObject> EditorActionReceived;

        protected static Messenger StartMessenger(string uri)
        {
            if (_mesenger == null)
            {
                SocketHandler socketHandler = new SocketHandler(uri);
                socketHandler.ChatMessageReceived += ChatMessageReceived;
                socketHandler.EditorActionReceived += EditorActionReceived;
                _mesenger = new Messenger(socketHandler);
            }
            return _mesenger;
        }
    }
}