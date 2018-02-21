using System;
using System.Linq;
using System.Windows.Ink;
using Newtonsoft.Json;
using PolyPaint.Constants;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Helpers.Communication
{
    public class Messenger
    {
        private readonly ISocketHandler _socketHandler;
        public bool IsConnected => _socketHandler.IsConnected;

        public Messenger(ISocketHandler socketHandler)
        {
            _socketHandler = socketHandler;
        }

        /// <summary>
        ///     Allows disconnecting and closing the socket
        /// </summary>
        public void DisconnectMessenger()
        {
            _socketHandler.DisconnectSocket();
        }

        /// <summary>
        ///     Allows reconnecting the socket after disconnecting it
        /// </summary>
        public void ReconnectMessenger()
        {
            if (!IsConnected) _socketHandler.ConnectSocket();
        }

        public static int DrawingRoomId { get; set; }
        /// <summary>
        ///     Builds a ChatMessageModel for a new message and sends it to the server
        /// </summary>
        /// <param name="outgoingMessage">String of the message to broadcast</param>
        /// <returns>Stringified JSON object if sending was successful, else returns an empty string</returns>
        public string SendChatMessage(string outgoingMessage)
        {
            if (outgoingMessage != string.Empty)
            {
                ChatMessageModel chatMessage = new ChatMessageModel
                {
                    Type = JsonConstantStrings.TypeChatMessageOutgoingValue,
                    Message = outgoingMessage,
                    Room = new RoomModel
                    {
                        Id = "chat"
                    }
                };

                string messageSerialized = JsonConvert.SerializeObject(chatMessage);

                bool isSent = _socketHandler.SendMessage(messageSerialized);

                if (isSent)
                    return messageSerialized;
            }
            return string.Empty;
        }

        /// <summary>
        ///     Builds an EditorActionModel for a new stroke and sends it to the server
        /// </summary>
        /// <param name="stroke">Newly added stroke</param>
        /// <returns>Stringified JSON object if sending was successful, else returns an empty string</returns>
        public string SendEditorActionNewStroke(Stroke stroke)
        {
            if (stroke != null)
            {
                EditorActionModel outgoingNewStrokeAction = new EditorActionModel
                {
                    Type = JsonConstantStrings.TypeEditorActionValue,
                    Drawing = new DrawingModel {Id = DrawingRoomId},
                    Action = new StrokeActionModel
                    {
                        Id = (int) ActionIds.NewStroke,
                        Name = Enum.GetName(typeof(ActionIds), ActionIds.NewStroke)
                    },
                    Stroke = new StrokeModel
                    {
                        DrawingAttributes = new DrawingAttributesModel
                        {
                            Color = stroke.DrawingAttributes.Color.ToString(),
                            Height = stroke.DrawingAttributes.Height,
                            Width = stroke.DrawingAttributes.Width,
                            StylusTip = stroke.DrawingAttributes.StylusTip.ToString()
                        },
                        Dots = stroke.StylusPoints
                                     .Select(point => new StylusPointModel {x = point.X, y = point.Y}).ToArray()
                    }
                };

                string actionSerialized = JsonConvert.SerializeObject(outgoingNewStrokeAction);

                bool isSent = _socketHandler.SendMessage(actionSerialized);

                if (isSent)
                    return actionSerialized;
            }
            return string.Empty;
        }
    }
}