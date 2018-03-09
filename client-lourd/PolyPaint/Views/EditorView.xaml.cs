﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.Models;
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
            //
            if (!_start.Equals(_end) && (DataContext as EditorViewModel)?.ToolSelected == "lasso")
            {
                //new adorner
                //add adorner
                //give strokes to the adorner

                //Add the rotating strokes adorner to the InkPresenter.
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(surfaceDessin);
                RotatingStrokesAdorner adorner = new RotatingStrokesAdorner(surfaceDessin);

                adornerLayer.Add(adorner);
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

        // Button.Click event handler that rotates the strokes
        private void RotateStrokes(object sender, RoutedEventArgs e)
        {
            StrokeCollection copiedStrokes = surfaceDessin.Strokes.Clone();
            Matrix rotatingMatrix = new Matrix();
            //double canvasLeft = Canvas.GetLeft(surfaceDessin);
            //double canvasTop = Canvas.GetTop(surfaceDessin);
            Point rotatePoint = new Point(surfaceDessin.ActualWidth / 2, surfaceDessin.ActualHeight / 2);

            rotatingMatrix.RotateAt(90, rotatePoint.X, rotatePoint.Y);
            copiedStrokes.Transform(rotatingMatrix, false);
            surfaceDessin.Strokes = copiedStrokes;

        }
        /* //new. On trying
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Add the rotating strokes adorner to the InkPresenter.
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(inkPresenter1);
            RotatingStrokesAdorner adorner = new RotatingStrokesAdorner(inkPresenter1);

            adornerLayer.Add(adorner);
        }*/
    }
}
