using System;
using System.Linq;
using System.Threading;
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

        [TestInitialize]
        public void ResetEditor()
        {
            _editor.StrokesCollection.Clear();
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
        }

        [TestMethod]
        public void TestAddNewStrokeCurrentUser()
        {
            string actionStr = _messenger.SendEditorActionNewStroke(_stroke);
            EditorActionModel action = JsonConvert.DeserializeObject<EditorActionModel>(actionStr);

            action.Author = new AuthorModel
            {
                Username = _editor.CurrentUsername
            };

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);

            Assert.AreEqual(0, _editor.StrokesCollection.Count, "Editor's StrokeCollection should be empty");
        }

        [TestMethod]
        public void TestRemoveStrokeSuccess()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            _editor.StrokesCollection.Add(_stroke);

            string actionStr = _messenger.SendEditorActionRemoveStroke(_stroke);
            EditorActionModel action = JsonConvert.DeserializeObject<EditorActionModel>(actionStr);

            action.Author = new AuthorModel
            {
                Username = "me@me.ca"
            };

            _editor.StrokesCollection.StrokesChanged += (s, e) => autoResetEvent.Set();

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);

            bool strokesChanged = autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsTrue(strokesChanged, "StrokeCollection should have changed");
            Assert.AreEqual(0, _editor.StrokesCollection.Count, "StrokeCollection should be empty");
        }

        [TestMethod]
        public void TestRemoveStrokeCurrentUser()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            _editor.StrokesCollection.Add(_stroke);

            string actionStr = _messenger.SendEditorActionRemoveStroke(_stroke);
            EditorActionModel action = JsonConvert.DeserializeObject<EditorActionModel>(actionStr);

            action.Author = new AuthorModel
            {
                Username = _editor.CurrentUsername
            };

            _editor.StrokesCollection.StrokesChanged += (s, e) => autoResetEvent.Set();

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);

            bool strokesChanged = autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsFalse(strokesChanged, "StrokeCollection should not have changed");
            Assert.AreEqual(1, _editor.StrokesCollection.Count, "StrokeCollection should not be empty");

            _editor.StrokesCollection.Clear();
        }

        [TestMethod]
        public void TestReplaceStroke()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            _editor.StrokesCollection.Add(_stroke);

            CustomStroke stroke1 = new CustomStroke(new StylusPointCollection(new[] {new StylusPoint(3, 7)}));

            CustomStroke stroke2 = new CustomStroke(new StylusPointCollection(new[] {new StylusPoint(1, 3)}));

            StrokeCollection strokes = new StrokeCollection {stroke1, stroke2};

            string actionStr = _messenger.SendEditorActionReplaceStroke(new[] {Guid.Empty.ToString()}, strokes);
            EditorActionModel action = JsonConvert.DeserializeObject<EditorActionModel>(actionStr);

            action.Author = new AuthorModel
            {
                Username = "me@me.ca"
            };

            _editor.StrokesCollection.StrokesChanged += (s, e) => autoResetEvent.Set();

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);

            bool strokesChanged = autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsTrue(strokesChanged, "StrokeCollection should have changed");

            Assert.AreEqual(2, _editor.StrokesCollection.Count, "StrokeCollection should contain 2 strokes");
        }

        [TestMethod]
        public void TestTransformStrokeSuccess()
        {
            _editor.StrokesCollection.Add(_stroke);

            Matrix transformation = new Matrix();
            transformation.Rotate(90);

            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
            {
                points.Add(new StylusPoint(i + 1, i + 1));
            }

            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            Stroke transformedStroke = new CustomStroke(points, attributes)
            {
                Uuid = Guid.Empty
            };

            transformedStroke.Transform(transformation, false);

            string actionStr =
                _messenger.SendEditorActionTransformedStrokes(new StrokeCollection(new[] {transformedStroke}));
            EditorActionModel action = JsonConvert.DeserializeObject<EditorActionModel>(actionStr);

            action.Author = new AuthorModel
            {
                Username = "me@me.ca"
            };

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);
            context.ExecuteStrategy(_editor);

            Assert.AreEqual(1, _editor.StrokesCollection.Count, "StrokeCollection should still contains 1 stroke");
            Assert.IsTrue(_editor.StrokesCollection.First().StylusPoints.SequenceEqual(transformedStroke.StylusPoints),
                          "Stroke in StrokeCollection should have the same points as the transformed stroke");
        }
    }
}
