using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    /// <summary>
    ///     Logique d'interaction pour EditorView.xaml
    /// </summary>
    public partial class EditorView : Window
    {
        private Point _end;

        //Starting and ending point of the mouse during an action
        private Point _start;

        public EditorView()
        {
            InitializeComponent();
            DataContext = new EditorViewModel();
            ((EditorViewModel)DataContext).LockedStrokesSelectedEvent += OnLockedStrokesSelectedEventHandler;
        }

        private void OnLockedStrokesSelectedEventHandler(object sender, StrokeCollection lockedStrokes)
        {
            StrokeCollection allSelectedStrokes = DrawingSurface.GetSelectedStrokes();
            allSelectedStrokes.Remove(lockedStrokes);

            DrawingSurface.Select(allSelectedStrokes);
        }

        // Pour gérer les points de contrôles.
        private void GlisserCommence(object sender, DragStartedEventArgs e)
        {
            (sender as Thumb).Background = Brushes.Black;
        }

        private void GlisserTermine(object sender, DragCompletedEventArgs e)
        {
            (sender as Thumb).Background = Brushes.White;
        }

        private void GlisserMouvementRecu(object sender, DragDeltaEventArgs e)
        {
            string nom = (sender as Thumb).Name;
            if (nom == "horizontal" || nom == "diagonal")
            {
                colonne.Width = new GridLength(Math.Max(32, colonne.Width.Value + e.HorizontalChange));
            }

            if (nom == "vertical" || nom == "diagonal")
            {
                ligne.Height = new GridLength(Math.Max(32, ligne.Height.Value + e.VerticalChange));
            }
        }

        // Pour la gestion de l'affichage de position du pointeur.
        private void surfaceDessin_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockPosition.Text = "";
        }

        private void surfaceDessin_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(DrawingSurface);
            textBlockPosition.Text = Math.Round(p.X) + ", " + Math.Round(p.Y) + "px";

            // Update the X & Y as the mouse moves
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _end = p;
            }

            //Transform the cursor in a cross when the tool is shapes
            if ((DataContext as EditorViewModel)?.ToolSelected == "shapes")
            {
                DrawingSurface.UseCustomCursor = true;
                DrawingSurface.Cursor = Cursors.Cross;
            }
            else
            {
                DrawingSurface.UseCustomCursor = false;
            }
        }

        private void SurfaceDessin_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //The shape is added and selected in our StrokeCollection in the release of the left mouse
            if (!_start.Equals(_end) && (DataContext as EditorViewModel)?.ToolSelected == "shapes")
            {
                StrokeCollection selectedShape = (DataContext as EditorViewModel).AddShape(_start, _end);
                DrawingSurface.Select(selectedShape);
            }
        }

        private void SurfaceDessin_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //The Dynamic Renderer is updated on each click
            DrawingSurface.CustomRenderer.SetViewModel(DataContext as EditorViewModel);

            //The position of the mouse is saved
            _start = e.GetPosition(DrawingSurface);
        }

        private void DupliquerSelection(object sender, RoutedEventArgs e)
        {
            DrawingSurface.CopySelection();
            DrawingSurface.Paste();
        }

        private void SupprimerSelection(object sender, RoutedEventArgs e)
        {
            DrawingSurface.CutSelection();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            (DataContext as EditorViewModel)?.OnStrokeCollectedHandler(sender, e);
        }

        private void OnStrokeErasing(object sender, InkCanvasStrokeErasingEventArgs e)
        {
            (DataContext as EditorViewModel)?.OnStrokeErasingHandler(sender, e);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            (DataContext as EditorViewModel)?.OnSelectionChangedHandler(DrawingSurface?.GetSelectedStrokes());
        }

        private void OnTextToInsertLostFocus(object sender, RoutedEventArgs e)
        {
            (DataContext as EditorViewModel)?.OnTextToInsertLostFocusHandler(TextToInsert, e);
        }

        private void OnSelectionResized(object sender, EventArgs e)
        {
            (DataContext as EditorViewModel)?.OnSelectionTransformedHandler(DrawingSurface?.GetSelectedStrokes());
        }

        private void OnSelectionMoved(object sender, EventArgs e)
        {
            (DataContext as EditorViewModel)?.OnSelectionTransformedHandler(DrawingSurface?.GetSelectedStrokes());
        }
    }
}
