using System.Windows.Controls;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    /// <summary>
    ///     Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            InitializeComponent();
            DataContext = new ChatViewModel();
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
