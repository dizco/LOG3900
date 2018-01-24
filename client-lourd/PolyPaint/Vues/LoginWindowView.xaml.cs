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
            DataContext = new VueModeles.LoginWindowViewModel();
        }
        
        private void SwitchToSignup(object sender, RoutedEventArgs e)
        {
            CanvasLogin.Visibility = System.Windows.Visibility.Hidden;
            CanvasSignup.Visibility = System.Windows.Visibility.Visible;
        }

        private void SwitchToLogin(object sender, RoutedEventArgs e)
        {
            CanvasSignup.Visibility = System.Windows.Visibility.Hidden;
            CanvasLogin.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
