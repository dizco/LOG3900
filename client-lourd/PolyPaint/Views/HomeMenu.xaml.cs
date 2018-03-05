using System.Windows;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    /// <summary>
    ///     Interaction logic for HomeMenu.xaml
    /// </summary>
    public partial class HomeMenu : Window
    {
        public HomeMenu()
        {
            InitializeComponent();
            DataContext = new HomeMenuViewModel();
        }

        private void TextBox_TextChanged(object sender)
        {

        }
    }
}
