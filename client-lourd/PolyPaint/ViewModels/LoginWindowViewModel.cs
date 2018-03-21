using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolyPaint.Helpers;
using PolyPaint.Helpers.Communication;
using PolyPaint.Views;

namespace PolyPaint.ViewModels
{
    internal class LoginWindowViewModel : ViewModelBase
    {
        private readonly CookieContainer _cookies;
        private string _serverUri;

        public LoginWindowViewModel()
        {
            LoginCommand = new RelayCommand<object>(Login);
            SignupCommand = new RelayCommand<StackPanel>(Signup);
            OfflineCommand = new RelayCommand<object>(SkipLogin);
            _cookies = new CookieContainer();
            RestHandler.Handler.CookieContainer = _cookies;
        }

        private string HttpServerUri => "http://" + ServerUri;
        private string WsServerUri => "ws://" + ServerUri;

        public string UserEmail { get; set; }

        public string Password { get; set; }

        public string ServerUri
        {
            get => _serverUri;
            set => _serverUri = FormatServerUri(value);
        }

        public RelayCommand<object> LoginCommand { get; set; }
        public RelayCommand<StackPanel> SignupCommand { get; set; }
        public RelayCommand<object> OfflineCommand { get; set; }
        public event EventHandler ClosingRequest;

        /// <summary>
        ///     Attempts to login. Returns void instead of Task so that Login(object o) doesn't have to be an async method
        /// </summary>
        private async void TryLoginRequest()
        {
            RestHandler.ServerUri = HttpServerUri;

            if (!await RestHandler.ValidateServerUri())
            {
                UserAlerts.ShowErrorMessage("L'adresse spécifiée n'est pas valide. \nL'adresse du serveur doit avoir la forme suivante : \nXXX.XXX.XXX.XXX:5025");
                return;
            }

            HttpResponseMessage response = await RestHandler.LoginUser(UserEmail, Password);

            if (response.IsSuccessStatusCode)
            {
                Username = UserEmail;
                OpenConnection();
            }
            else
            {
                OnResponseError(await response.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        ///     Attemps to register. Returns void instead of Task so that Register(object o) doesn't have to be an async method
        /// </summary>
        private async void TryRegisterRequest()
        {
            RestHandler.ServerUri = HttpServerUri;

            if (!await RestHandler.ValidateServerUri())
            {
                UserAlerts.ShowErrorMessage("L'adresse spécifiée n'est pas valide. \nL'adresse du serveur doit avoir la forme suivante : \nXXX.XXX.XXX.XXX:5025");
                return;
            }

            HttpResponseMessage response = await RestHandler.RegisterInfo(UserEmail, Password);

            if (response.IsSuccessStatusCode)
            {
                Username = UserEmail;
                OpenConnection();
            }
            else
            {
                OnResponseError(await response.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        ///     Gets the list of cookies that were issued by the server for the current user
        /// </summary>
        private List<KeyValuePair<string, string>> GetCookiesAsList()
        {
            IEnumerable<Cookie> responseCookies =
                _cookies.GetCookies(new Uri(RestHandler.ServerUri)).Cast<Cookie>();

            return responseCookies
                   .Select(cookie => new KeyValuePair<string, string>(cookie.Name, cookie.Value)).ToList();
        }

        /// <summary>
        ///     Opens the websocket and chat
        /// </summary>
        private void OpenConnection()
        {
            List<KeyValuePair<string, string>> cookies = GetCookiesAsList();
            StartMessenger(WsServerUri, cookies);
            OpenHomeMenu();
        }

        /// <summary>
        ///     Removes schema from URI entered by user so that appropriate schema can be concatenated for REST and WebSocket
        ///     protocols
        /// </summary>
        public static string FormatServerUri(string uri)
        {
            string uriStripPattern = @"^http:\/\/|^ws:\/\/";
            return Regex.Replace(uri, uriStripPattern, "");
        }

        /// <summary>
        ///     Parses server response and display appropriate error message
        /// </summary>
        /// <param name="response">Serialized JSON response</param>
        private void OnResponseError(string response)
        {
            JObject responseJson;
            try
            {
                responseJson = JObject.Parse(response);
            }
            catch (JsonReaderException e)
            {
                UserAlerts.ShowErrorMessage(e.Message);
                return;
            }

            UserAlerts.ShowErrorMessage(responseJson.GetValue("error").ToString());
        }

        private void Login(object o)
        {
            Password = (o as PasswordBox)?.Password;
            TryLoginRequest();
        }

        private void Signup(StackPanel passwordContainer)
        {
            string firstPassword = (passwordContainer.Children[1] as PasswordBox)?.Password;
            string confirmPassword = (passwordContainer.Children[3] as PasswordBox)?.Password;
            if (firstPassword?.Equals(confirmPassword) ?? false)
            {
                Password = firstPassword;
            }
            else
            {
                UserAlerts.ShowErrorMessage("Les mots de passe ne sont pas identiques.");
                return;
            }

            TryRegisterRequest();
        }

        private void SkipLogin(object obj)
        {
            OpenHomeMenu();
        }

        private void OpenHomeMenu()
        {
            if (HomeMenu == null)
            {
                HomeMenu = new HomeMenu();
                HomeMenu.Show();
                HomeMenu.Closing += (s, a) => HomeMenu = null;
                OnClosingRequest();
            }
            else
            {
                HomeMenu.Show();
                OnClosingRequest();
            }
        }

        /// <summary>
        ///     Raises ClosingRequest to trigger the closing of the LoginWindowView
        /// </summary>
        private void OnClosingRequest()
        {
            ClosingRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
