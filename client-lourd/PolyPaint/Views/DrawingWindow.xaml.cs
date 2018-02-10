﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    /// <summary>
    ///     Logique d'interaction pour DrawingWindow.xaml
    /// </summary>
    public partial class DrawingWindow : Window
    {
        public DrawingWindow()
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
                colonne.Width = new GridLength(Math.Max(32, colonne.Width.Value + e.HorizontalChange));
            if (nom == "vertical" || nom == "diagonal")
                ligne.Height = new GridLength(Math.Max(32, ligne.Height.Value + e.VerticalChange));
        }

        // Pour la gestion de l'affichage de position du pointeur.
        private void surfaceDessin_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlockPosition.Text = "";
        }

        private void surfaceDessin_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(surfaceDessin);
            textBlockPosition.Text = Math.Round(p.X) + ", " + Math.Round(p.Y) + "px";
        }

        private void DupliquerSelection(object sender, RoutedEventArgs e)
        {
            surfaceDessin.CopySelection();
            surfaceDessin.Paste();
        }

        private void SupprimerSelection(object sender, RoutedEventArgs e)
        {
            surfaceDessin.CutSelection();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
        }

        //Shutdown all the app when the user close the main window
        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            (DataContext as EditorViewModel)?.OnStrokeCollectedHandler(sender, e);
        }

        private void OnStrokeErasing(object sender, InkCanvasStrokeErasingEventArgs e)
        {
            //TODO: needs implementation
            //(DataContext as VueModele)?.OnStrokeErasingHandler(sender, e);
        }

        private void OnStrokeErased(object sender, RoutedEventArgs e)
        {
            //TODO: needs implementation
            //(DataContext as VueModele)?.OnStrokeErased(sender, e);
        }
    }
}