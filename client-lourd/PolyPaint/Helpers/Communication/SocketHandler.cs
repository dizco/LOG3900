using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolyPaint.Constants;
using PolyPaint.Models.MessagingModels;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace PolyPaint.Helpers.Communication
{
    public class SocketHandler : ISocketHandler
    {
        private readonly WebSocket _ws;

        public SocketHandler(string uri, List<KeyValuePair<string, string>> cookies)
        {
            _ws = new WebSocket(uri, string.Empty, cookies);
            _ws.Opened += OnOpened;
            _ws.Error += OnError;
            _ws.Closed += OnClosed;
            _ws.MessageReceived += OnMessageReceived;
            ConnectSocket();
        }

        public bool IsConnected { get; private set; }

        public void ConnectSocket()
        {
            _ws.Open();
        }

        public void DisconnectSocket()
        {
            _ws.Close();
            // TODO: Add disconnect reason.
        }

        public bool SendMessage(string data)
        {
            if (IsConnected)
            {
                _ws.Send(data);
                return true;
            }
            return false;
        }

        public event EventHandler<ChatMessageModel> ChatMessageReceived;
        public event EventHandler<EditorActionModel> EditorActionReceived;

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            JObject incomingData;
            try
            {
                incomingData = JObject.Parse(e.Message);
            }
            catch
            {
                // ignored
                return;
            }
            string type = incomingData.GetValue("type").ToString();
            if (type == JsonConstantStrings.TypeChatMessageIncomingValue)
                OnChatMessageReceived(e.Message);
            else if (type == JsonConstantStrings.TypeEditorActionValue)
                OnEditorActionReceived(e.Message);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            IsConnected = false;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            // TODO: Implemented reconnection logic
            ErrorEventArgs args = e;
            IsConnected = false;
        }

        private void OnOpened(object sender, EventArgs e)
        {
            IsConnected = true;
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