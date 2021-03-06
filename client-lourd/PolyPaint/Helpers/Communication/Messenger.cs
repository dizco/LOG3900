﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using Newtonsoft.Json;
using PolyPaint.Constants;
using PolyPaint.Converters;
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

        public string SubscribeToDrawing()
        {
            SubscriptionMessageModel subscribe = new SubscriptionMessageModel
            {
                Type = JsonConstantStrings.TypeEditorSubscriptionOutgoingValue,
                Drawing = new DrawingModel
                {
                    Id = DrawingRoomId
                },
                Action = new SubscriptionMessageModel.SubscribeAction
                {
                    Id = SubscriptionAction.Join.GetDescription()
                }
            };

            string actionSerialized = JsonConvert.SerializeObject(subscribe);

            bool isSent = SendDrawingAction(actionSerialized);

            return isSent ? actionSerialized : string.Empty;
        }

        public string UnsubscribeToDrawing()
        {
            SubscriptionMessageModel subscribe = new SubscriptionMessageModel
            {
                Type = JsonConstantStrings.TypeEditorSubscriptionOutgoingValue,
                Drawing = new DrawingModel
                {
                    Id = DrawingRoomId
                },
                Action = new SubscriptionMessageModel.SubscribeAction
                {
                    Id = SubscriptionAction.Leave.GetDescription()
                }
            };

            string actionSerialized = JsonConvert.SerializeObject(subscribe);

            bool isSent = SendDrawingAction(actionSerialized);

            return isSent ? actionSerialized : string.Empty;
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
        ///     Sends the strokeAction to the server if the drawing is an online drawing (has a DrawingId)
        /// </summary>
        /// <param name="actionSerialized">Serialized strokeAction to send to the server</param>
        /// <returns>Boolean representing sent status.</returns>
        private bool SendDrawingAction(string actionSerialized)
        {
            return DrawingRoomId != null && _socketHandler.SendMessage(actionSerialized);
        }

        private StrokeEditorActionModel BuildOutgoingStrokeAction(StrokeActionIds strokeAction)
        {
            return new StrokeEditorActionModel
            {
                Type = JsonConstantStrings.TypeStrokeEditorActionOutgoingValue,
                Drawing = new DrawingModel {Id = DrawingRoomId},
                Action = new StrokeActionModel
                {
                    Id = (int) strokeAction,
                    Name = Enum.GetName(typeof(StrokeActionIds), strokeAction)
                }
            };
        }

        private PixelEditorActionModel BuildOutgoingPixelAction(PixelActionIds action)
        {
            return new PixelEditorActionModel
            {
                Type = JsonConstantStrings.TypePixelEditorActionOutgoingValue,
                Drawing = new DrawingModel {Id = DrawingRoomId},
                Action = new PixelActionModel
                {
                    Id = (int) action,
                    Name = Enum.GetName(typeof(PixelActionIds), action)
                }
            };
        }

        /// <summary>
        ///     Builds an StrokeEditorActionModel for a new stroke and sends it to the server
        /// </summary>
        /// <param name="stroke">Newly added stroke</param>
        /// <returns>Stringified JSON object if sending was successful, else returns an empty string</returns>
        internal string SendEditorActionNewStroke(CustomStroke stroke)
        {
            if (stroke != null)
            {
                StrokeEditorActionModel outgoingNewStrokeAction = BuildOutgoingStrokeAction(StrokeActionIds.NewStroke);
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

        internal string SendEditorActionNewStrokes(List<StrokeModel> strokes)
        {
            if (strokes.Any())
            {
                StrokeEditorActionModel outgoingNewStrokeAction = BuildOutgoingStrokeAction(StrokeActionIds.NewStroke);
                outgoingNewStrokeAction.Delta = new DeltaModel
                {
                    Add = strokes.ToArray()
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
        ///     Builds an StrokeEditorActionModel for a stroke that was stacked by the current user and sends the strokeAction to
        ///     the server
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
                StrokeEditorActionModel outgoingRemoveStrokeAction =
                    BuildOutgoingStrokeAction(StrokeActionIds.ReplaceStroke);

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
                    }).ToArray() ?? new StrokeModel[0]
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

        internal string SendEditorActionLockStrokes(List<string> strokes)
        {
            return SendEditorActionLockUnlockStrokes(strokes, StrokeActionIds.LockStrokes);
        }

        internal string SendEditorActionUnlockStrokes(List<string> strokes)
        {
            return SendEditorActionLockUnlockStrokes(strokes, StrokeActionIds.UnlockStrokes);
        }

        private string SendEditorActionLockUnlockStrokes(List<string> strokes, StrokeActionIds strokeAction)
        {
            if (strokes?.Count > 0 && (strokeAction == StrokeActionIds.LockStrokes ||
                                       strokeAction == StrokeActionIds.UnlockStrokes))
            {
                StrokeEditorActionModel outgoingLockStrokesAction = BuildOutgoingStrokeAction(strokeAction);

                outgoingLockStrokesAction.Delta = new DeltaModel
                {
                    Remove = strokes.ToArray()
                };

                string actionSerialized = JsonConvert.SerializeObject(outgoingLockStrokesAction);

                bool isSent = SendDrawingAction(actionSerialized);

                if (isSent)
                {
                    return actionSerialized;
                }
            }

            return string.Empty;
        }

        internal string SendEditorActionTransformedStrokes(StrokeCollection strokes)
        {
            if (strokes.Count > 0)
            {
                StrokeEditorActionModel outgoingTransformStrokesAction =
                    BuildOutgoingStrokeAction(StrokeActionIds.Transform);

                outgoingTransformStrokesAction.Delta = new DeltaModel
                {
                    Add = strokes.Select(stroke => new StrokeModel
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

                string actionSerialized = JsonConvert.SerializeObject(outgoingTransformStrokesAction);

                bool isSent = SendDrawingAction(actionSerialized);

                if (isSent)
                {
                    return actionSerialized;
                }
            }

            return string.Empty;
        }

        internal string SendEditorActionResetDrawing()
        {
            StrokeEditorActionModel outgoingResetAction = BuildOutgoingStrokeAction(StrokeActionIds.Reset);
            outgoingResetAction.Delta = new DeltaModel();

            string actionSerialized = JsonConvert.SerializeObject(outgoingResetAction);

            return SendDrawingAction(actionSerialized) ? actionSerialized : string.Empty;
        }

        internal string SendEditorActionNewPixels(List<Tuple<Point, string>> newPixels)
        {
            if (newPixels.Count <= 1)
            {
                return string.Empty;
            }

            PixelEditorActionModel outgoingNewPixels = BuildOutgoingPixelAction(PixelActionIds.NewPixels);

            outgoingNewPixels.Pixels = newPixels
                                       .Select(pixel => new PixelModel
                                       {
                                           X = pixel.Item1.X,
                                           Y = pixel.Item1.Y,
                                           Color = pixel.Item2
                                       }).Where(pixel => pixel.X >= 0 && pixel.Y >= 0).ToArray();

            string actionSerialized = JsonConvert.SerializeObject(outgoingNewPixels);

            return SendDrawingAction(actionSerialized) ? actionSerialized : string.Empty;
        }
    }
}
