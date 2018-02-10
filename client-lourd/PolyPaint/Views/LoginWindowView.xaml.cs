using System.Windows;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    /// <summary>
    ///     Logique d'interaction pour LoginWindow1.xaml
    /// </summary>
    public partial class LoginWindowView : Window
    {
        public LoginWindowView()
        {
            InitializeComponent();
            DataContext = new LoginWindowViewModel();
            (DataContext as LoginWindowViewModel).ClosingRequest += (sender, e) => Close();
        }

        private void SwitchToSignup(object sender, RoutedEventArgs e)
        {
            CanvasLogin.Visibility = Visibility.Hidden;
            CanvasSignup.Visibility = Visibility.Visible;
        }

        private void SwitchToLogin(object sender, RoutedEventArgs e)
        {
            CanvasSignup.Visibility = Visibility.Hidden;
            CanvasLogin.Visibility = Visibility.Visible;
        }
    }
}