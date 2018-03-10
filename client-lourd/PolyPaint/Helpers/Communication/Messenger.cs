using System;
using System.Linq;
using System.Windows.Ink;
using Newtonsoft.Json;
using PolyPaint.Constants;
using PolyPaint.CustomComponents;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Helpers.Communication
{
    public class Messenger
    {
        private readonly ISocketHandler _socketHandler;

        public Messenger(ISocketHandler socketHandler)
        {
            _socketHandler = socketHandler;
        }

        public bool IsConnected => _socketHandler.IsConnected;

        public static string DrawingRoomId { get; set; }

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
            if (!IsConnected)
            {
                _socketHandler.ConnectSocket();
            }
        }

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
                {
                    return messageSerialized;
                }
            }

            return string.Empty;
        }

        /// <summary>
        ///     Sends the action to the server if the drawing is an online drawing (has a DrawingId)
        /// </summary>
        /// <param name="actionSerialized">Serialized action to send to the server</param>
        /// <returns>Boolean representing sent status.</returns>
        private bool SendDrawingAction(string actionSerialized)
        {
            return DrawingRoomId != null && _socketHandler.SendMessage(actionSerialized);
        }

        private EditorActionModel BuildOutgoingAction(ActionIds action)
        {
            return new EditorActionModel
            {
                Type = JsonConstantStrings.TypeEditorActionOutgoingValue,
                Drawing = new DrawingModel {Id = DrawingRoomId},
                Action = new StrokeActionModel
                {
                    Id = (int) action,
                    Name = Enum.GetName(typeof(ActionIds), action)
                }
            };
        }

        /// <summary>
        ///     Builds an EditorActionModel for a new stroke and sends it to the server
        /// </summary>
        /// <param name="stroke">Newly added stroke</param>
        /// <returns>Stringified JSON object if sending was successful, else returns an empty string</returns>
        internal string SendEditorActionNewStroke(CustomStroke stroke)
        {
            if (stroke != null)
            {
                EditorActionModel outgoingNewStrokeAction = BuildOutgoingAction(ActionIds.NewStroke);
                outgoingNewStrokeAction.Delta = new DeltaModel
                {
                    Add = new[]
                    {
                        new StrokeModel
                        {
                            Uuid = stroke?.Uuid.ToString(),
                            DrawingAttributes = new DrawingAttributesModel
                            {
                                Color = stroke.DrawingAttributes.Color.ToString(),
                                Height = stroke.DrawingAttributes.Height,
                                Width = stroke.DrawingAttributes.Width,
                                StylusTip = stroke.DrawingAttributes.StylusTip.ToString()
                            },
                            Dots = stroke.StylusPoints
                                         .Select(point => new StylusPointModel {X = point.X, Y = point.Y}).ToArray()
                        }
                    }
                };

                string actionSerialized = JsonConvert.SerializeObject(outgoingNewStrokeAction);

                bool isSent = SendDrawingAction(actionSerialized);

                if (isSent)
                {
                    return actionSerialized;
                }
            }

            return string.Empty;
        }

        /// <summary>
        ///     Builds an EditorActionModel for a stroke that was stacked by the current user and sends the action to the server
        /// </summary>
        /// <param name="stroke">Stroke that has just been put on the stack</param>
        /// <returns>Stringified JSON object if sending was successful, else returns an empty string</returns>
        internal string SendEditorActionRemoveStroke(CustomStroke stroke)
        {
            return SendEditorActionReplaceStroke(new[] {stroke.Uuid.ToString()});
        }

        public string SendEditorActionReplaceStroke(string[] remove, StrokeCollection add = null)
        {
            if (remove.Length > 0)
            {
                EditorActionModel outgoingRemoveStrokeAction = BuildOutgoingAction(ActionIds.ReplaceStroke);

                outgoingRemoveStrokeAction.Delta = new DeltaModel
                {
                    Remove = remove,
                    Add = add?.Select(stroke => new StrokeModel
                    {
                        Uuid = (stroke as CustomStroke)?.Uuid.ToString(),
                        DrawingAttributes = new DrawingAttributesModel
                        {
                            Color = stroke.DrawingAttributes.Color.ToString(),
                            Height = stroke.DrawingAttributes.Height,
                            Width = stroke.DrawingAttributes.Width,
                            StylusTip = stroke.DrawingAttributes.StylusTip.ToString()
                        },
                        Dots = stroke.StylusPoints
                                     .Select(point => new StylusPointModel {X = point.X, Y = point.Y}).ToArray()
                    }).ToArray()
                };

                string actionSerialized = JsonConvert.SerializeObject(outgoingRemoveStrokeAction);

                bool isSent = SendDrawingAction(actionSerialized);

                if (isSent)
                {
                    return actionSerialized;
                }
            }

            return string.Empty;
        }
    }
}
