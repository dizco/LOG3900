using System.Windows;
using System.Windows.Controls;
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

        private void ScrollViewScrollToBottom(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange > 0)
            {
                (sender as ScrollViewer)?.ScrollToEnd();
            }
        }
    }
}
