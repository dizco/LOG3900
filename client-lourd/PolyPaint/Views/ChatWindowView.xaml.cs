using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    /// <summary>
    ///     Interaction logic for ChatWindowView.xaml
    /// </summary>
    public partial class ChatWindowView : Window
    {
        public ChatWindowView()
        {
            InitializeComponent();
            DataContext = new ChatWindowViewModel();
        }

        private void MessageInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression binding = ((TextBox) sender).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
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