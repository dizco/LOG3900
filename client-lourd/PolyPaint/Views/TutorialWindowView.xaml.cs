using System.Windows;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    /// <summary>
    /// Logique d'interaction pour TutorialWindowView.xaml
    /// </summary>
    public partial class TutorialWindowView : Window
    {
        public TutorialWindowView(string tutorialMode)
        {
            InitializeComponent();
            DataContext = new TutorialWindowViewModel(tutorialMode);
        }
    }
}
