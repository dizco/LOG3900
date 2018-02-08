using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using PolyPaint.Modeles;
using PolyPaint.Modeles.MessagingModels;
using PolyPaint.Utilitaires;

namespace PolyPaint.VueModeles
{
    /// <summary>
    ///     Expose the commands and properties connected with the model to the elements of the ChatWindowView
    ///     Receive changes from the model and send the Send Requests to the View
    /// </summary>
    internal class ChatWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private static readonly object Lock = new object();
        private readonly ChatWindowModel _chatWindowModel = new ChatWindowModel();

        //Constructor
        public ChatWindowViewModel()
        {
            Items = new ObservableCollection<ChatMessage>();
            BindingOperations.EnableCollectionSynchronization(Items, Lock);
            ChatMessageReceived += DisplayReceivedMessage;

            //Sending a message 
            SendMessageCommand = new RelayCommand<object>(SendMessage);
        }

        public RelayCommand<object> SendMessageCommand { get; }

        //Contain the information of all message sent in the chatroom
        public ObservableCollection<ChatMessage> Items { get; set; }

        //Atribute defining the string message send by a user in the chat
        public string PendingChatMessage
        {
            get => _chatWindowModel.PendingChatMessage;
            set
            {
                _chatWindowModel.PendingChatMessage = value;
                PropertyModified();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SendMessage(object o)
        {
            Messenger.SendChatMessage(PendingChatMessage);

            //Sending all the information about the item
            if (PendingChatMessage != string.Empty && Messenger.IsConnected)
            {
                AppendMessageToChat(PendingChatMessage);
                //clear message after it's transmission
                PendingChatMessage = string.Empty;
            }
        }

        private void DisplayReceivedMessage(object sender, ChatMessageModel message)
        {
            AppendMessageToChat(string.Empty, message);
        }

        private void AppendMessageToChat(string outgoingMessage = "", ChatMessageModel incomingMessage = null)
        {
            string message, author;
            DateTime messageTime;
            bool sentByMe;

            if (outgoingMessage != string.Empty)
            {
                message = outgoingMessage;
                messageTime = DateTime.UtcNow.ToLocalTime();
                author = "Me";
                sentByMe = true;
            }
            else if (incomingMessage != null)
            {
                DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                message = incomingMessage.Message;
                messageTime = unixEpoch.AddMilliseconds(incomingMessage.Timestamp).ToLocalTime();
                sentByMe = false;
                author = incomingMessage.Author?.Name ?? "PodMuncher";
                sentByMe = false;
            }
            else
            {
                //Invalid call to method, exit before processing any further
                return;
            }

            lock (Lock)
            {
                Items.Add(new ChatMessage
                {
                    Title = message,
                    MessageSentTime = messageTime,
                    SenderName = author,
                    SentByMe = sentByMe,
                    NewItem = true
                });
            }
        }

        /// <summary>
        ///     Call when a property of the ViewModel is changed
        ///     An event is send by the Viewmodel then
        ///     The event contains the name of the property modified and will be catched by the View
        ///     The View will than update the componant concerned
        /// </summary>
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //Temporary fake text message
    internal class ChatMessage
    {
        public string Title { get; set; } = string.Empty;
        public DateTime MessageSentTime { get; set; } = DateTime.UtcNow;
        public bool SentByMe { get; set; }
        public string SenderName { get; set; } = "missingno";
        public bool NewItem { get; set; } = true;
    }
}