using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolyPaint.Constants;
using PolyPaint.Modeles.MessagingModels;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace PolyPaint.Utilitaires
{
    public class SocketHandler : ISocketHandler
    {
        private readonly WebSocket ws;
        private bool isConnected;

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

        public event EventHandler<ChatMessageModel> ChatMessageReceived;
        public event EventHandler<EditorActionModel> EditorActionReceived;

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            JObject incomingData = JObject.Parse(e.Message);
            string type = incomingData.GetValue("type").ToString();
            if (type == JsonConstantStrings.TypeChatMessageIncomingValue)
                OnChatMessageReceived(e.Message);
            else if (type == JsonConstantStrings.TypeEditorActionValue)
                OnEditorActionReceived(e.Message);
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

        public void OnChatMessageReceived(string e)
        {
            ChatMessageModel messageDeserialized = JsonConvert.DeserializeObject<ChatMessageModel>(e);
            ChatMessageReceived?.Invoke(this, messageDeserialized);
        }

        public void OnEditorActionReceived(string e)
        {
            EditorActionModel actionDeserialized = JsonConvert.DeserializeObject<EditorActionModel>(e);
            EditorActionReceived?.Invoke(this, actionDeserialized);
        }
    }
}