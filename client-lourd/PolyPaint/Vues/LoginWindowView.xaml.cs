using System.Windows;
namespace PolyPaint.Vues
{
    /// <summary>
    /// Logique d'interaction pour LoginWindow1.xaml
    /// </summary>
    public partial class LoginWindowView : Window
    {
        public LoginWindowView()
        {
            InitializeComponent();
            DataContext = new VueModeles.LoginWindowViewModele();
        }

        private void switchToSignUp(object sender, RoutedEventArgs e)
        {
            canvasLogIn.Visibility = System.Windows.Visibility.Hidden;
            canvasSignUp.Visibility = System.Windows.Visibility.Visible;
        }
        private void switchToLogIn(object sender, RoutedEventArgs e)
        {
            canvasSignUp.Visibility = System.Windows.Visibility.Hidden;
            canvasLogIn.Visibility = System.Windows.Visibility.Visible;
        }

        private void logIn(object sender, RoutedEventArgs e)
        {
            Page1 chatWindow = new Page1();
            chatWindow.Show();
            this.Close();
        }

        private void signUp(object sender, RoutedEventArgs e)
        {
            Page1 chatWindow = new Page1();
            chatWindow.Show();
            this.Close();
        }
    }
}
