using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Logique d'interaction pour SignUpView.xaml
    /// </summary>

    public partial class SignUpView : Window
    {
        public SignUpView()
        {
            InitializeComponent();
        }

        private void contentVis(object sender, RoutedEventArgs e)
        {
            haut.Visibility = System.Windows.Visibility.Visible;
        }

        private void contentHid(object sender, RoutedEventArgs e)
        {
            haut.Visibility = System.Windows.Visibility.Hidden;
        }

        private void contentCol(object sender, RoutedEventArgs e)
        {
            haut.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
