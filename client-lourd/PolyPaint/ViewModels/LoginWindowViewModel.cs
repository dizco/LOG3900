using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
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
            SignupCommand = new RelayCommand<object>(Signup);
            ShowErrorMessageCommand = new RelayCommand<string>(ShowMessageBox);
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
        public RelayCommand<object> SignupCommand { get; set; }
        public RelayCommand<string> ShowErrorMessageCommand { get; set; }
        public event EventHandler ClosingRequest;

        /// <summary>
        ///     Attempts to login. Returns void instead of Task so that Login(object o) doesn't have to be an async method
        /// </summary>
        private async void TryLoginRequest()
        {
            RestHandler.ServerUri = HttpServerUri;
            
            if (!await RestHandler.ValidateServerUri())
            {
                ShowErrorMessageCommand.Execute("L'adresse spécifiée n'est pas valide. \nL'adresse du serveur doit avoir la forme suivante : \nXXX.XXX.XXX.XXX:5025");
                return;
            }

            HttpResponseMessage response = await RestHandler.LoginInfo(UserEmail, Password);

            if (response.IsSuccessStatusCode)
            {
                OpenConnection();
                Username = UserEmail;
            }
            else
                OnResponseError(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        ///     Attemps to register. Returns void instead of Task so that Register(object o) doesn't have to be an async method
        /// </summary>
        private async void TryRegisterRequest()
        {
            RestHandler.ServerUri = HttpServerUri;
            
            if (!await RestHandler.ValidateServerUri())
            {
                ShowErrorMessageCommand.Execute("L'adresse spécifiée n'est pas valide. \nL'adresse du serveur doit avoir la forme suivante : \nXXX.XXX.XXX.XXX:5025");
                return;
            }

            HttpResponseMessage response = await RestHandler.RegisterInfo(UserEmail, Password);

            if (response.IsSuccessStatusCode)
            {
                OpenConnection();
                Username = UserEmail;
            }
            else
                OnResponseError(await response.Content.ReadAsStringAsync());
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
            OpenChatWindow();
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
                ShowErrorMessageCommand.Execute(e.Message);
                return;
            }

            ShowErrorMessageCommand.Execute(responseJson.GetValue("error").ToString());
        }

        private void Login(object o)
        {
            Password = (o as PasswordBox)?.Password;
            TryLoginRequest();
        }

        private void Signup(object o)
        {
            Password = (o as PasswordBox)?.Password;
            TryRegisterRequest();
        }

        /// <summary>
        ///     Implementation of the SHowMessageBoxCommand for displaying an error message to the user
        /// </summary>
        /// <param name="errorMessage">Error message to be displayed</param>
        private void ShowMessageBox(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OpenChatWindow()
        {
            if (ChatWindow == null)
            {
                ChatWindow = new ChatWindowView();
                ChatWindow.Show();
                ChatWindow.Closed += (sender, args) => ChatWindow = null;
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