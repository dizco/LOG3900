using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.CustomComponents;
using PolyPaint.Helpers.Communication;

namespace PolyPaintTests.Helpers
{
    [TestClass]
    public class MessengerTest
    {
        private const string DummyServerUri = "ws://localhost:3000";
        private static Messenger _messenger;
        private static Messenger _messengerFail;

        [ClassInitialize]
        public static void InitializeMessenger(TestContext context)
        {
            _messenger = new Messenger(new SocketHandlerMock(DummyServerUri));
            _messengerFail = new Messenger(new SocketHandlerMockFail(DummyServerUri));
        }

        [TestMethod]
        public void TestSendChatMessageEmpty()
        {
            const string testString = "";

            string realOutputString = _messenger.SendChatMessage(testString);

            Assert.AreEqual(testString, realOutputString,
                            "An empty string in input should return an empty string as output");
        }

        [TestMethod]
        public void TestSendChatMessageSuccess()
        {
            const string testMessage = "This is a test message";

            const string expectedOutputString = "{\"message\":\"" + testMessage +
                                                "\",\"author\":null,\"room\":{\"id\":\"chat\",\"name\":null},\"timestamp\":0,\"type\":\"client.chat.message\"}";

            string realOutputString = _messenger.SendChatMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString, "Should return JSON-formatted string");
        }

        [TestMethod]
        public void TestSendChatMessageWithQuotes()
        {
            const string testMessage = "This is a test message containing \" quotes \"";

            const string expectedOutputString =
                "{\"message\":\"This is a test message containing \\\" quotes \\\"\",\"author\":null,\"room\":{\"id\":\"chat\",\"name\":null},\"timestamp\":0,\"type\":\"client.chat.message\"}";

            string realOutputString = _messenger.SendChatMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return JSON-formatted string without breaking due to quotes");
        }

        [TestMethod]
        public void TestSendChatMessageWithSocketFail()
        {
            const string testMessage = "The content of the message doesn't matter";

            const string expectedOutputString = "";

            string realOutputString = _messengerFail.SendChatMessage(testMessage);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return an empty string because SocketHandler failed to send");
        }

        [TestMethod]
        public void TestSendEditorActionNewStrokeWithInvalidStroke()
        {
            object stroke = new object();
            string expectedOutputString = string.Empty;

            string realOutputString = _messenger.SendEditorActionNewStroke(stroke as CustomStroke);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return empty string since stroke is not of type stroke/is null");
        }

        [TestMethod]
        public void TestSendEditorActionNewStrokeSuccess()
        {
            Messenger.DrawingRoomId = "drawingId";

            string expectedOutputString =
                "{\"action\":{\"id\":1,\"name\":\"NewStroke\"},\"author\":null,\"drawing\":{\"id\":\"SomeRoom\"},\"delta\":{\"add\":[{\"strokeUuid\":\"00000000-0000-0000-0000-000000000000\",\"strokeAttributes\":{\"color\":\"#FF000000\",\"height\":2.0031496062992127,\"width\":2.0031496062992127,\"stylusTip\":\"Ellipse\"},\"dots\":[{\"x\":1.0,\"y\":1.0},{\"x\":2.0,\"y\":2.0},{\"x\":3.0,\"y\":3.0},{\"x\":4.0,\"y\":4.0},{\"x\":5.0,\"y\":5.0},{\"x\":6.0,\"y\":6.0},{\"x\":7.0,\"y\":7.0},{\"x\":8.0,\"y\":8.0},{\"x\":9.0,\"y\":9.0},{\"x\":10.0,\"y\":10.0}]}],\"remove\":[]},\"layer\":0,\"type\":\"client.editor.stroke.action\"}";

            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            CustomStroke stroke = new CustomStroke(points, attributes)
            {
                Uuid = Guid.Empty
            };

            Messenger.DrawingRoomId = "SomeRoom";

            string realOutputString = _messenger.SendEditorActionNewStroke(stroke);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of an StrokeEditorActionModel");
        }

        [TestMethod]
        public void TestSendEditorActionNewStrokeNoDrawingId()
        {
            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            CustomStroke stroke = new CustomStroke(points, attributes);

            Messenger.DrawingRoomId = null;
            string realOutputString = _messenger.SendEditorActionNewStroke(stroke);

            Assert.AreEqual(string.Empty, realOutputString,
                            "Should return stringified JSON of an StrokeEditorActionModel");
        }

        [TestMethod]
        public void TestSendEditorActionStackSuccess()
        {
            Messenger.DrawingRoomId = "drawingId";

            string expectedOutputString =
                "{\"action\":{\"id\":2,\"name\":\"ReplaceStroke\"},\"author\":null,\"drawing\":{\"id\":\"drawingId\"},\"delta\":{\"add\":[],\"remove\":[\"00000000-0000-0000-0000-000000000000\"]},\"layer\":0,\"type\":\"client.editor.stroke.action\"}";

            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            CustomStroke stroke = new CustomStroke(points, attributes, "Author", Guid.Empty.ToString());

            string realOutputString = _messenger.SendEditorActionRemoveStroke(stroke);

            Messenger.DrawingRoomId = null;
            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of an StrokeEditorActionModel");
        }

        [TestMethod]
        public void TestSendEditorActionStackSuccessNoDrawingId()
        {
            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            CustomStroke stroke = new CustomStroke(points, attributes);
            string realOutputString = _messenger.SendEditorActionRemoveStroke(stroke);

            Messenger.DrawingRoomId = null;
            Assert.AreEqual(string.Empty, realOutputString,
                            "Should return stringified JSON of an StrokeEditorActionModel");
        }

        [TestMethod]
        public void TestEditorSendActionReplaceStroke()
        {
            Messenger.DrawingRoomId = "drawing";

            string expectedOutputString =
                "{\"action\":{\"id\":2,\"name\":\"ReplaceStroke\"},\"author\":null,\"drawing\":{\"id\":\"drawing\"},\"delta\":{\"add\":[{\"strokeUuid\":\"00000000-0000-0000-0000-000000000000\",\"strokeAttributes\":{\"color\":\"#FF000000\",\"height\":2.0031496062992127,\"width\":2.0031496062992127,\"stylusTip\":\"Ellipse\"},\"dots\":[{\"x\":1.0,\"y\":1.0},{\"x\":2.0,\"y\":2.0},{\"x\":3.0,\"y\":3.0},{\"x\":4.0,\"y\":4.0},{\"x\":5.0,\"y\":5.0},{\"x\":6.0,\"y\":6.0},{\"x\":7.0,\"y\":7.0},{\"x\":8.0,\"y\":8.0},{\"x\":9.0,\"y\":9.0},{\"x\":10.0,\"y\":10.0}]},{\"strokeUuid\":\"00000000-0000-0000-0000-000000000000\",\"strokeAttributes\":{\"color\":\"#FF000000\",\"height\":2.0031496062992127,\"width\":2.0031496062992127,\"stylusTip\":\"Ellipse\"},\"dots\":[{\"x\":11.0,\"y\":11.0},{\"x\":12.0,\"y\":12.0},{\"x\":13.0,\"y\":13.0},{\"x\":14.0,\"y\":14.0},{\"x\":15.0,\"y\":15.0},{\"x\":16.0,\"y\":16.0},{\"x\":17.0,\"y\":17.0},{\"x\":18.0,\"y\":18.0},{\"x\":19.0,\"y\":19.0},{\"x\":20.0,\"y\":20.0}]}],\"remove\":[\"00000000-0000-0000-0000-000000000000\"]},\"layer\":0,\"type\":\"client.editor.stroke.action\"}";

            StylusPointCollection points1 = new StylusPointCollection();
            StylusPointCollection points2 = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points1.Add(new StylusPoint(i + 1, i + 1));
                points2.Add(new StylusPoint(i + 11, i + 11));
            }

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            CustomStroke stroke1 = new CustomStroke(points1, attributes)
            {
                Uuid = Guid.Empty
            };
            CustomStroke stroke2 = new CustomStroke(points2, attributes)
            {
                Uuid = Guid.Empty
            };
            StrokeCollection strokes = new StrokeCollection
            {
                stroke1,
                stroke2
            };

            string realOutputString = _messenger.SendEditorActionReplaceStroke(new[] {Guid.Empty.ToString()}, strokes);

            Messenger.DrawingRoomId = null;
            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of an EditorActionmodel");
        }

        [TestMethod]
        public void TestSendEditorActionNewStrokeWithSocketFail()
        {
            string expectedOutputString = string.Empty;

            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            CustomStroke stroke = new CustomStroke(points, attributes);

            string realOutputString = _messengerFail.SendEditorActionNewStroke(stroke);

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return an empty string because SocketHandler failed to send message");
        }

        [TestMethod]
        public void TestSendEditorActionLockStrokes()
        {
            Messenger.DrawingRoomId = "Room";

            Guid strokeUuid1 = Guid.NewGuid();
            Guid strokeUuid2 = Guid.NewGuid();

            string expectedOutputString =
                "{\"action\":{\"id\":3,\"name\":\"LockStrokes\"},\"author\":null,\"drawing\":{\"id\":\"Room\"},\"delta\":{\"add\":[],\"remove\":[\"" +
                strokeUuid1 + "\",\"" + strokeUuid2 + "\"]},\"layer\":0,\"type\":\"client.editor.stroke.action\"}";

            string realOutputString =
                _messenger.SendEditorActionLockStrokes(new List<string>(new[]
                {
                    strokeUuid1.ToString(), strokeUuid2.ToString()
                }));

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of an StrokeEditorActionModel");
        }

        [TestMethod]
        public void TestSendEditorActionLockStrokesNoDrawingId()
        {
            Messenger.DrawingRoomId = null;

            Guid strokeUuid1 = Guid.NewGuid();
            Guid strokeUuid2 = Guid.NewGuid();

            string expectedOutputString = string.Empty;

            string realOutputString =
                _messenger.SendEditorActionLockStrokes(new List<string>(new[]
                {
                    strokeUuid1.ToString(), strokeUuid2.ToString()
                }));

            Assert.AreEqual(expectedOutputString, realOutputString, "Should return an empty string");
        }

        [TestMethod]
        public void TestSendEditorActionUnLockStrokes()
        {
            Messenger.DrawingRoomId = "Room";

            Guid strokeUuid1 = Guid.NewGuid();
            Guid strokeUuid2 = Guid.NewGuid();

            string expectedOutputString =
                "{\"action\":{\"id\":4,\"name\":\"UnlockStrokes\"},\"author\":null,\"drawing\":{\"id\":\"Room\"},\"delta\":{\"add\":[],\"remove\":[\"" +
                strokeUuid1 + "\",\"" + strokeUuid2 + "\"]},\"layer\":0,\"type\":\"client.editor.stroke.action\"}";

            string realOutputString =
                _messenger.SendEditorActionUnlockStrokes(new List<string>(new[]
                {
                    strokeUuid1.ToString(), strokeUuid2.ToString()
                }));

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of an StrokeEditorActionModel");
        }

        [TestMethod]
        public void TestSendEditorActionTransformStroke()
        {
            Messenger.DrawingRoomId = "Room";

            string expectedOutputString =
                "{\"action\":{\"id\":5,\"name\":\"Transform\"},\"author\":null,\"drawing\":{\"id\":\"Room\"},\"delta\":{\"add\":[{\"strokeUuid\":\"00000000-0000-0000-0000-000000000000\",\"strokeAttributes\":{\"color\":\"#FF000000\",\"height\":2.0031496062992127,\"width\":2.0031496062992127,\"stylusTip\":\"Ellipse\"},\"dots\":[{\"x\":1.0,\"y\":1.0},{\"x\":2.0,\"y\":2.0},{\"x\":3.0,\"y\":3.0},{\"x\":4.0,\"y\":4.0},{\"x\":5.0,\"y\":5.0},{\"x\":6.0,\"y\":6.0},{\"x\":7.0,\"y\":7.0},{\"x\":8.0,\"y\":8.0},{\"x\":9.0,\"y\":9.0},{\"x\":10.0,\"y\":10.0}]}],\"remove\":[]},\"layer\":0,\"type\":\"client.editor.stroke.action\"}";

            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            CustomStroke stroke = new CustomStroke(points, attributes)
            {
                Uuid = Guid.Empty
            };

            string realOutputString =
                _messenger.SendEditorActionTransformedStrokes(new StrokeCollection(new[] {stroke}));

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of an StrokeEditorActionModel");
        }

        [TestMethod]
        public void TestSendEditorActionTransformNoStroke()
        {
            Messenger.DrawingRoomId = "Room";

            string expectedOutputString = string.Empty;

            string realOutputString = _messenger.SendEditorActionTransformedStrokes(new StrokeCollection());

            Assert.AreEqual(expectedOutputString, realOutputString, "Should return empty string");
        }

        [TestMethod]
        public void TestSendEditorActionTransformNoDrawingId()
        {
            Messenger.DrawingRoomId = null;

            string expectedOutputString = string.Empty;

            //Generate stylus points
            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            //Generate drawing attributes
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            //Create stroke
            CustomStroke stroke = new CustomStroke(points, attributes)
            {
                Uuid = Guid.Empty
            };

            string realOutputString =
                _messenger.SendEditorActionTransformedStrokes(new StrokeCollection(new[] {stroke}));

            Assert.AreEqual(expectedOutputString, realOutputString, "Should return empty string");
        }

        [TestMethod]
        public void TestSendEditorSubscribeDrawing()
        {
            Messenger.DrawingRoomId = "room";

            string expectedOutputString =
                "{\"drawing\":{\"id\":\"room\"},\"action\":{\"id\":\"join\",\"name\":\"\"},\"type\":\"client.editor.subscription\"}";

            string realOutputString = _messenger.SubscribeToDrawing();

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of Subscription action");
        }

        [TestMethod]
        public void TestSendEditorUnsubscribeDrawing()
        {
            Messenger.DrawingRoomId = "room";

            string expectedOutputString =
                "{\"drawing\":{\"id\":\"room\"},\"action\":{\"id\":\"leave\",\"name\":\"\"},\"type\":\"client.editor.subscription\"}";

            string realOutputString = _messenger.UnsubscribeToDrawing();

            Assert.AreEqual(expectedOutputString, realOutputString,
                            "Should return stringified JSON of Subscription action");
        }

        [TestMethod]
        public void TestSendEditorPixels()
        {
            Messenger.DrawingRoomId = "room";

            string exptectedOutputString =
                "{\"action\":{\"id\":1,\"name\":\"NewPixels\"},\"author\":null,\"drawing\":{\"id\":\"room\"},\"pixels\":[{\"x\":0.0,\"y\":0.0,\"color\":\"black\"},{\"x\":1.0,\"y\":1.0,\"color\":\"black\"},{\"x\":2.0,\"y\":2.0,\"color\":\"black\"},{\"x\":3.0,\"y\":3.0,\"color\":\"black\"},{\"x\":4.0,\"y\":4.0,\"color\":\"black\"},{\"x\":5.0,\"y\":5.0,\"color\":\"black\"},{\"x\":6.0,\"y\":6.0,\"color\":\"black\"},{\"x\":7.0,\"y\":7.0,\"color\":\"black\"},{\"x\":8.0,\"y\":8.0,\"color\":\"black\"},{\"x\":9.0,\"y\":9.0,\"color\":\"black\"}],\"type\":\"client.editor.pixel.action\"}";

            List<Tuple<Point, string>> pixels = new List<Tuple<Point, string>>();
            for (int i = 0; i < 10; i++)
            {
                pixels.Add(new Tuple<Point, string>(new Point(i, i), "black"));
            }

            string realOutputString = _messenger.SendEditorActionNewPixels(pixels);

            Assert.AreEqual(exptectedOutputString, realOutputString,
                            "Should return stringified JSON of outgoing pixels");
        }

        internal class SocketHandlerMock : ISocketHandler
        {
            public SocketHandlerMock(string uri)
            {
            }

            public bool IsConnected { get; } = true;

            public void DisconnectSocket()
            {
            }

            public void ConnectSocket()
            {
            }

            public bool SendMessage(string data)
            {
                return true;
            }
        }

        internal class SocketHandlerMockFail : ISocketHandler
        {
            public SocketHandlerMockFail(string uri)
            {
            }

            public bool IsConnected { get; } = true;

            public void DisconnectSocket()
            {
            }

            public void ConnectSocket()
            {
            }

            public bool SendMessage(string data)
            {
                return false;
            }
        }
    }
}
