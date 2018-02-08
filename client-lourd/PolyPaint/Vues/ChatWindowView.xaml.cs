using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using PolyPaint.VueModeles;

namespace PolyPaint.Vues
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

        private void GridSplitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
        }

        private void MessageInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression binding = ((TextBox) sender).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnClosing(CancelEventArgs e)
        {

            base.OnClosing(e);
        }
    }
}