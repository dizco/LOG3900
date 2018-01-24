using PolyPaint.VueModeles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Logique d'interaction pour ChatWindowView.xaml
    /// </summary>
    public partial class ChatWindowView : Window
    {

        public ChatWindowView()
        {
            InitializeComponent();
            DataContext = new VueModeles.ChatWindowViewModele();
           
        }
        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void MessageInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}