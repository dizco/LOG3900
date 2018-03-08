using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using PolyPaint.ViewModels;

namespace PolyPaint.CustomComponents
{
    public class CustomRenderingInkCanvas : InkCanvas
    {
        internal readonly CustomDynamicRenderer CustomRenderer = new CustomDynamicRenderer();

        public CustomRenderingInkCanvas()
        {
            DynamicRenderer = CustomRenderer;
        }

        // TODO: Raise the same event so that EditorViewModel can call the messenger with the appropriate info
        //protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        //{
        //    Strokes.Remove(e.Stroke);
        //    CustomStroke customStroke = new CustomStroke(e.Stroke.StylusPoints, e.Stroke.DrawingAttributes);
        //    Strokes.Add(customStroke);
        //}

        internal class CustomDynamicRenderer : DynamicRenderer
        {
            private static EditorViewModel _viewModel = new EditorViewModel();
            private Point _start;

            private bool IsManipulating { get; set; }

            public void SetViewModel(EditorViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            protected override void OnStylusDown(RawStylusInput rawStylusInput)
            {
                //get the position of the first stylus when we start drawing
                _start = (Point) rawStylusInput.GetStylusPoints()[0];
                base.OnStylusDown(rawStylusInput);
            }

            protected override void OnDraw(DrawingContext drawingContext,
                StylusPointCollection stylusPoints,
                Geometry geometry, Brush fillBrush)
            {
                if (_viewModel.ToolSelected == "shapes")
                {
                    //The renderer is reset after each move of the stylus
                    //we then see only one shape rendered
                    if (!IsManipulating)
                    {
                        IsManipulating = true;
                        StylusDevice currentStylus = Stylus.CurrentStylusDevice;
                        Reset(currentStylus, stylusPoints);
                    }

                    IsManipulating = false;
                }

                // Draw forms between all the StylusPoints that have come in.
                int lastStylus = stylusPoints.Count - 1;
                Point end = (Point) stylusPoints[lastStylus];

                if (_viewModel.ToolSelected == "shapes")
                {
                    //Draw a shape when the tool shapes is selected
                    drawingContext.DrawGeometry(fillBrush, null, _viewModel.DrawShape(_start, end).GetGeometry());
                }
                else
                {
                    //The stylus wont be overrided
                    base.OnDraw(drawingContext, stylusPoints, geometry, fillBrush);
                }
            }
        }
    }
}
