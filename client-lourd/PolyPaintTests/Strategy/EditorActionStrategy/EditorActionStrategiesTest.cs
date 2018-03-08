using System;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.Constants;
using PolyPaint.CustomComponents;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Strategy.EditorActionStrategy;

namespace PolyPaintTests.Strategy.EditorActionStrategy
{
    [TestClass]
    public class EditorActionStrategiesTest
    {
        private static Editor _editor;
        private static Stroke _stroke;

        [ClassInitialize]
        public static void InitializeMessenger(TestContext context)
        {
            _editor = new Editor();

            StylusPointCollection points = new StylusPointCollection();
            for (int i = 0; i < 10; i++)
                points.Add(new StylusPoint(i + 1, i + 1));
            DrawingAttributes attributes = new DrawingAttributes {Color = Colors.Black};

            _stroke = new CustomStroke(points, attributes)
            {
                Uuid = Guid.Empty.ToString()
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
            EditorActionModel action = new EditorActionModel
            {
                Type = JsonConstantStrings.TypeEditorActionOutgoingValue,
                Drawing = new DrawingModel {Id = "507f1f77bcf86cd799439011"},
                Author = new AuthorModel
                {
                    Username = "me@me.ca"
                },
                Action = new StrokeActionModel
                {
                    Id = (int) ActionIds.NewStroke,
                    Name = Enum.GetName(typeof(ActionIds), ActionIds.NewStroke)
                },
                Delta = new DeltaModel
                {
                    Add = new[]
                    {
                        new StrokeModel
                        {
                            Uuid = Guid.Empty.ToString(),
                            DrawingAttributes = new DrawingAttributesModel
                            {
                                Color = _stroke.DrawingAttributes.Color.ToString(),
                                Height = _stroke.DrawingAttributes.Height,
                                Width = _stroke.DrawingAttributes.Width,
                                StylusTip = _stroke.DrawingAttributes.StylusTip.ToString()
                            },
                            Dots = _stroke.StylusPoints
                                          .Select(point => new StylusPointModel {X = point.X, Y = point.Y}).ToArray()
                        }
                    }
                }
            };

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);

            Assert.AreEqual(_editor.StrokesCollection.Count, 1, "Editor's StrokeCollection should contain 1 stroke");

            Assert.AreEqual(_stroke.DrawingAttributes, _editor.StrokesCollection[0].DrawingAttributes,
                            "Newly added stroke should have the same DrawingAttributes");

            Assert.IsTrue(_stroke.StylusPoints.SequenceEqual(_editor.StrokesCollection[0].StylusPoints),
                          "Newly added stroke should contain the same StylusPoints as initial stroke");

            Assert.AreEqual("me@me.ca", (_editor.StrokesCollection[0] as CustomStroke)?.Author,
                            "Newly added stroke should have the same author");
        }
    }
}
