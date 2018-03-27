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
        public enum DisconnectionReason
        {
            Errored = -1,
            Closed = 0
        }

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
        public event EventHandler<StrokeEditorActionModel> StrokeEditorActionReceived;
        public event EventHandler<PixelEditorActionModel> PixelEditorActionReceived;
        public event EventHandler EditorPollRequestReceived;
        public event EventHandler<int> WebSocketDisconnectedEvent;
        public event EventHandler WebSocketConnectedEvent;

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
            {
                OnChatMessageReceived(e.Message);
            }
            else if (type == JsonConstantStrings.TypeStrokeEditorActionIncomingValue)
            {
                OnStrokeEditorActionReceived(e.Message);
            }
            else if (type == JsonConstantStrings.TypePixelEditorActionIncomingValue)
            {
                OnPixelEditorActionReceived(e.Message);
            }
            else if (type == JsonConstantStrings.TypeEditorPollIncomingValue)
            {
                OnEditorPollRequestReceived();
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            WebSocketDisconnected((int) DisconnectionReason.Closed);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            WebSocketDisconnected((int) DisconnectionReason.Errored);
        }

        private void OnOpened(object sender, EventArgs e)
        {
            WebSocketConnected();
        }

        public void OnChatMessageReceived(string e)
        {
            ChatMessageModel messageDeserialized = JsonConvert.DeserializeObject<ChatMessageModel>(e);
            ChatMessageReceived?.Invoke(this, messageDeserialized);
        }

        public void OnStrokeEditorActionReceived(string e)
        {
            StrokeEditorActionModel actionDeserialized = JsonConvert.DeserializeObject<StrokeEditorActionModel>(e);
            StrokeEditorActionReceived?.Invoke(this, actionDeserialized);
        }

        private void OnPixelEditorActionReceived(string e)
        {
            PixelEditorActionModel actionDeserialized = JsonConvert.DeserializeObject<PixelEditorActionModel>(e);
            PixelEditorActionReceived?.Invoke(this, actionDeserialized);
        }

        private void OnEditorPollRequestReceived()
        {
            EditorPollRequestReceived?.Invoke(this, null);
        }

        public void WebSocketDisconnected(int e)
        {
            IsConnected = false;
            WebSocketDisconnectedEvent?.Invoke(this, e);
        }

        public void WebSocketConnected()
        {
            IsConnected = true;
            WebSocketConnectedEvent?.Invoke(this, null);
        }
    }
}
