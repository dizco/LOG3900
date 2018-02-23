using System;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyPaint.Constants;
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

            _stroke = new Stroke(points, attributes);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidActionStrategyException))]
        public void TestInvalidEditorAction()
        {
            EditorActionModel action = new EditorActionModel
            {
                Type = JsonConstantStrings.TypeEditorActionOutgoingValue,
                Drawing = new DrawingModel {Id = 0},
                Action = new StrokeActionModel
                {
                    Id = -1,
                    Name = "FakeAction"
                },
                Stroke = new StrokeModel
                {
                    DrawingAttributes = new DrawingAttributesModel
                    {
                        Color = _stroke.DrawingAttributes.Color.ToString(),
                        Height = _stroke.DrawingAttributes.Height,
                        Width = _stroke.DrawingAttributes.Width,
                        StylusTip = _stroke.DrawingAttributes.StylusTip.ToString()
                    },
                    Dots = _stroke.StylusPoints
                                  .Select(point => new StylusPointModel {x = point.X, y = point.Y}).ToArray()
                }
            };

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);
        }

        [TestMethod]
        public void TestAddNewStroke()
        {
            EditorActionModel action = new EditorActionModel
            {
                Type = JsonConstantStrings.TypeEditorActionOutgoingValue,
                Drawing = new DrawingModel {Id = 0},
                Action = new StrokeActionModel
                {
                    Id = (int) ActionIds.NewStroke,
                    Name = Enum.GetName(typeof(ActionIds), ActionIds.NewStroke)
                },
                Stroke = new StrokeModel
                {
                    DrawingAttributes = new DrawingAttributesModel
                    {
                        Color = _stroke.DrawingAttributes.Color.ToString(),
                        Height = _stroke.DrawingAttributes.Height,
                        Width = _stroke.DrawingAttributes.Width,
                        StylusTip = _stroke.DrawingAttributes.StylusTip.ToString()
                    },
                    Dots = _stroke.StylusPoints
                                  .Select(point => new StylusPointModel {x = point.X, y = point.Y}).ToArray()
                }
            };

            EditorActionStrategyContext context = new EditorActionStrategyContext(action);

            context.ExecuteStrategy(_editor);

            Assert.AreEqual(_editor.StrokesCollection.Count, 1, "Editor's StrokeCollection should contain 1 stroke");

            Assert.AreEqual(_stroke.DrawingAttributes, _editor.StrokesCollection[0].DrawingAttributes, "Newly added stroke should have the same DrawingAttributes");

            bool ContainSameStylusPoints = true;

            for (int i = 0; i < _stroke.StylusPoints.Count; i++)
            {
                if (_stroke.StylusPoints[i] != _editor.StrokesCollection[0].StylusPoints[i])
                    ContainSameStylusPoints = false;
            }

            Assert.IsTrue(ContainSameStylusPoints, "Newly added stroke should contain the same StylusPoints as initial stroke");
        }
    }
}