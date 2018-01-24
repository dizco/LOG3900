using System;
using SuperSocket.ClientEngine;
using Newtonsoft.Json.Linq;
using WebSocket4Net;

namespace PolyPaint.Utilitaires
{

    public class SocketHandler : ISocketHandler
    {
        private WebSocket ws;
        private bool isConnected = false;

        public SocketHandler(string uri)
        {
            ws = new WebSocket(uri);
            ws.Opened += new EventHandler(OnOpened);
            ws.Error += new EventHandler<ErrorEventArgs>(OnError);
            ws.Closed += new EventHandler(OnClosed);
            ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(OnMessageReceived);
            ws.Open();
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            JObject incomingData = JObject.Parse(e.Message);
            // TODO: Verify message type and call function accordingly
        }

        private void OnClosed(object sender, EventArgs e)
        {
            isConnected = false;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            var args = e;
            isConnected = false;
            throw new NotImplementedException();
        }

        private void OnOpened(object sender, EventArgs e)
        {
            isConnected = true;
        }

        public bool SendMessage(string data)
        {
            if (isConnected)
            {
                ws.Send(data);
                return true;
            }
            return false;
        }
    }
}
