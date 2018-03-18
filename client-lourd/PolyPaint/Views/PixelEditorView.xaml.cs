﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    public partial class PixelEditorView : Window
    {
        private Point _newPosition;
        private Point _oldPosition;

        public PixelEditorView()
        {
            InitializeComponent();
            DataContext = new PixelEditorlViewModel();
        }

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

        private void DrawingSurfaceMouseEnter(object sender, MouseEventArgs e)
        {
            (DataContext as PixelEditorlViewModel)?.PixelCursors(DisplayArea);
            _oldPosition = e.GetPosition(DrawingSurface);
        }

        private void DrawingSurfacePreviewMouseDown(object sender, MouseEventArgs e)
        {
            _oldPosition = e.GetPosition(DrawingSurface);

            //The tool is selected on click with a distance of one pixel to
            //enable it
            Point onePixelPoint = new Point(_oldPosition.X + 1, _oldPosition.Y);
            (DataContext as PixelEditorlViewModel)?.PixelDraw(_oldPosition, onePixelPoint);
        }

        private void DrawingSurfaceMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Action tool on mouse move
                _newPosition = e.GetPosition(DrawingSurface);
                (DataContext as PixelEditorlViewModel)?.PixelDraw(_oldPosition, _newPosition);
                _oldPosition = _newPosition;
            }
        }

        private void DrawingSurfaceOnMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _newPosition = e.GetPosition(DrawingSurface);
                (DataContext as PixelEditorlViewModel)?.PixelDraw(_oldPosition, _newPosition);
            }
        }
    }
}