using System;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PolyPaint.Constants;
using PolyPaint.CustomComponents;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Strategy.EditorActionStrategy;
using PolyPaintTests.Helpers;

namespace PolyPaintTests.Strategy.EditorActionStrategy
{
    [TestClass]
    public class EditorActionStrategiesTest
    {
        private static Editor _editor;
        private static Messenger _messenger;
        private static CustomStroke _stroke;

        [ClassInitialize]
        public static void InitializeMessenger(TestContext context)
        {
            _editor = new Editor();
            _messenger = new Messenger(new MessengerTest.SocketHandlerMock("ws://something"));

            Messenger.DrawingRoomId = "507f1f77bcf86cd799439011";
            _editor.CurrentUsername = "me2@me.ca";

            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            _stroke = new CustomStroke(points, attributes)
            {
                Uuid = Guid.Empty
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidActionStrategyException))]
        public void TestInvalidEditorAction()
        {
            EditorActionModel action = new EditorActionModel
            {
                Type = JsonConstantStrings.TypeEditorActionOutgoingValue,
                Drawing = new DrawingModel {Id = "507f1f77bcf86cd799439011"},
                Action = new StrokeActionModel
                {
                    Id = -1,
                    Name = "FakeAction"
                }
            };

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);
        }

        [TestMethod]
        public void TestAddNewStrokeOtherUser()
        {
            string actionStr = _messenger.SendEditorActionNewStroke(_stroke);
            EditorActionModel action = JsonConvert.DeserializeObject<EditorActionModel>(actionStr);

            action.Author = new AuthorModel
            {
                Username = "me@me.ca"
            };

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);

            Assert.AreEqual(1, _editor.StrokesCollection.Count, "Editor's StrokeCollection should contain 1 stroke");

            Assert.AreEqual(_stroke.DrawingAttributes, _editor.StrokesCollection[0].DrawingAttributes,
                            "Newly added stroke should have the same DrawingAttributes");

            Assert.IsTrue(_stroke.StylusPoints.SequenceEqual(_editor.StrokesCollection[0].StylusPoints),
                          "Newly added stroke should contain the same StylusPoints as initial stroke");

            Assert.AreEqual("me@me.ca", (_editor.StrokesCollection[0] as CustomStroke)?.Author,
                            "Newly added stroke should have the same author");

            _editor.StrokesCollection.Clear();
        }

        [TestMethod]
        public void TestRemoveStrokeSuccess()
        {
            _editor.StrokesCollection.Add(_stroke);

            string actionStr = _messenger.SendEditorActionRemoveStroke(_stroke);
            EditorActionModel action = JsonConvert.DeserializeObject<EditorActionModel>(actionStr);

            action.Author = new AuthorModel
            {
                Username = "me@me.ca"
            };

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);

            Assert.AreEqual(0, _editor.StrokesCollection.Count, "StrokeCollection should be empty");

            _editor.StrokesCollection.Clear();
        }
    }
}
