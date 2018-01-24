using PolyPaint.Utilitaires;
using PolyPaint.Vues;

namespace PolyPaint.VueModeles
{
    internal class LoginWindowViewModel
    {
        //Command for managing the views
        public RelayCommand<object> LoginCommand { get; set; }
        public RelayCommand<object> SignupCommand { get; set; }
        public ChatWindowView ChatWindow { get; private set; }

        public LoginWindowViewModel()
        {
            LoginCommand = new RelayCommand<object>(Login);
            SignupCommand = new RelayCommand<object>(Signup);
        }

        private void Login(object o)
        {
            // TODO : implement Login

            OpenChatWindow();
        }

        private void Signup(object o)
        {
            // TODO : implement Signup

            OpenChatWindow();
        }

        private void OpenChatWindow()
        {
            if (ChatWindow == null)
            {
                ChatWindow = new Vues.ChatWindowView();
                ChatWindow.Show();
                ChatWindow = null;
            }
        }
    }
}