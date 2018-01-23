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
    }
}
