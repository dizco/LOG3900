using System;
using System.Linq;
using System.Windows.Ink;
using Newtonsoft.Json;
using PolyPaint.Constants;
using PolyPaint.Modeles.MessagingModels;

namespace PolyPaint.Utilitaires
{
    public class Messenger
    {
        private readonly ISocketHandler _socketHandler;

        public Messenger(ISocketHandler socketHandler)
        {
            _socketHandler = socketHandler;
        }

        public static string DrawingRoomId { get; set; }

        /// <summary>
        ///     This function builds a ChatMessageModel for a new message and sends it to the server
        /// </summary>
        /// <param name="outgoingMessage">String of the message to broadcast</param>
        /// <returns>Stringified JSON object if sending was successful, else returns an empty string</returns>
        public string SendChatMessage(string outgoingMessage)
        {
            if (outgoingMessage != string.Empty)
            {
                ChatMessageModel chatMessage = new ChatMessageModel
                {
                    type = JsonConstantStrings.TypeChatMessageOutgoingValue,
                    message = outgoingMessage,
                    room = new RoomModel
                    {
                        id = "chat"
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
        ///     This function builds an EditorActionModel for a new stroke and sends it to the server
        /// </summary>
        /// <param name="newStroke">Newly added stroke</param>
        /// <returns>Stringified JSON object if sending was successful, else returns an empty string</returns>
        public string SendEditorActionNewStroke(object newStroke)
        {
            if (newStroke is Stroke stroke)
            {
                EditorActionModel outgoingNewStrokeAction = new EditorActionModel
                {
                    type = JsonConstantStrings.TypeEditorActionValue,
                    drawing = new DrawingModel {id = DrawingRoomId},
                    action = new StrokeActionModel
                    {
                        id = (int) ActionIds.NewStroke,
                        name = Enum.GetName(typeof(ActionIds), ActionIds.NewStroke)
                    },
                    stroke = new StrokeModel
                    {
                        drawingAttributes = new DrawingAttributesModel
                        {
                            color = stroke.DrawingAttributes.Color.ToString(),
                            height = stroke.DrawingAttributes.Height,
                            width = stroke.DrawingAttributes.Width,
                            stylusTip = stroke.DrawingAttributes.StylusTip.ToString()
                        },
                        dots = stroke.StylusPoints
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