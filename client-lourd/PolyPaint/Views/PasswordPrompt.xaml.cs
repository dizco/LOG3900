using System;
using System.Windows;
using PolyPaint.Helpers;

namespace PolyPaint.Views
{
    /// <summary>
    ///     Interaction logic for PasswordPrompt.xaml
    /// </summary>
    public partial class PasswordPrompt : Window
    {
        public PasswordPrompt()
        {
            InitializeComponent();
        }

        public event EventHandler<string> PasswordEntered;

        private void OpenDrawingClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DrawingPassword.Password))
            {
                UserAlerts.ShowErrorMessage("Le mot de passe ne peut pas être vide");
            }
            else
            {
                PasswordEntered?.Invoke(this, DrawingPassword.Password);
            }
        }
    }
}
