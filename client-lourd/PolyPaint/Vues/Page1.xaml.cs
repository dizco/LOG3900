using System.Windows;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Logique d'interaction pour Page1.xaml
    /// </summary>
    public partial class Page1 : Window
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void goBack(object sender, RoutedEventArgs e)
        {
            LoginWindowView logInWindow = new LoginWindowView();
            logInWindow.Show();
            this.Close();
        }
    }
}
