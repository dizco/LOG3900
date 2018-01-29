using System;
using Newtonsoft.Json.Linq;
using PolyPaint.Constants;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace PolyPaint.Utilitaires
{
    public class SocketHandler : ISocketHandler
    {
        private readonly WebSocket ws;
        private bool isConnected;

        public event EventHandler<JObject> ChatMessageReceived;
        public event EventHandler<JObject> EditorActionReceived;

        public SocketHandler(string uri)
        {
            ws = new WebSocket(uri);
            ws.Opened += OnOpened;
            ws.Error += OnError;
            ws.Closed += OnClosed;
            ws.MessageReceived += OnMessageReceived;
            ws.Open();
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

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            JObject incomingData = JObject.Parse(e.Message);
            string type = incomingData.GetValue("type").ToString();
            if (type == JsonConstantStrings.TypeChatMessageIncomingValue)
            {
                // TODO: Display message
                OnChatMessageReceived(incomingData);
            }
            else if (type == JsonConstantStrings.TypeEditorActionValue)
            {
                // TODO: Manage incoming editor actions
                OnEditorActionReceived(incomingData);
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            isConnected = false;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            // TODO: Implemented reconnection logic
            ErrorEventArgs args = e;
            isConnected = false;
        }

        private void OnOpened(object sender, EventArgs e)
        {
            isConnected = true;
        }

        protected virtual void OnChatMessageReceived(JObject e)
        {
            ChatMessageReceived?.Invoke(this, e);
        }

        protected virtual void OnEditorActionReceived(JObject e)
        {
            EditorActionReceived?.Invoke(this, e);
        }
    }
}