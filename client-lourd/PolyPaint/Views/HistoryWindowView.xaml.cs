﻿using System.Windows;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    /// <summary>
    ///     Interaction logic for HistoryWindowView.xaml
    /// </summary>
    public partial class HistoryWindowView : Window
    {
        public HistoryWindowView()
        {
            InitializeComponent();
            DataContext = new HistoryWindowViewModel();
        }
    }
}
