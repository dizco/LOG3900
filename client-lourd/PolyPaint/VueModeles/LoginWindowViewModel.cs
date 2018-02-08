using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolyPaint.Utilitaires;
using PolyPaint.Vues;

namespace PolyPaint.VueModeles
{
    internal class LoginWindowViewModel : ViewModelBase
    {
        private string _serverUri;

        public LoginWindowViewModel()
        {
            LoginCommand = new RelayCommand<object>(Login);
            SignupCommand = new RelayCommand<object>(Signup);
        }

        public string UserEmail { get; set; }
        public string Password { get; set; } = "hahahaha";

        public string ServerUri
        {
            get => _serverUri;
            set => _serverUri = FormatServerUri(value);
        }

        //Command for managing the views
        public RelayCommand<object> LoginCommand { get; set; }

        public RelayCommand<object> SignupCommand { get; set; }
        public ChatWindowView ChatWindow { get; private set; }

        public event EventHandler ClosingRequest;

        private void Login(object o)
        {
            TryLoginRequest();
        }

        private void Signup(object o)
        {
            // TODO : implement Signup
            // OpenChatWindow();
        }

        private async Task TryLoginRequest()
        {
            RestHandler.ServerUri = "http://" + ServerUri;
            
            // TODO: Alert user of invalid URI
            if (!await RestHandler.ValidateServerUri())
                return;

            HttpResponseMessage response = await RestHandler.LoginInfo(UserEmail, Password);

            JObject responseJson;
            try
            {
                responseJson = JObject.Parse(await response.Content.ReadAsStringAsync());
            }
            catch (JsonReaderException e)
            {
                // TODO: Handle exception
                return;
            }
            if (responseJson.GetValue("status").ToString() == "success")
            {
                StartMessenger("ws://" + ServerUri);
                OpenChatWindow();
            }
            else
            {
                //TODO: Alert user of error
            }
        }

        public static string FormatServerUri(string uri)
        {
            string uriStripPattern = @"^http:|^ws:|\/";
            return Regex.Replace(uri, uriStripPattern, "");
        }

        private void OpenChatWindow()
        {
            if (ChatWindow == null)
            {
                ChatWindow = new ChatWindowView();
                ChatWindow.Show();
                ChatWindow = null;
                OnClosingRequest();
            }
        }

        private void OnClosingRequest()
        {
            ClosingRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}