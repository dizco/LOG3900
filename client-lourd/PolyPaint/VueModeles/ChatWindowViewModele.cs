using PolyPaint.Modeles;
using PolyPaint.Utilitaires;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PolyPaint.VueModeles
{

    /// <summary>
    /// Expose the commands and properties connected with the model to the elements of the ChatWindowView
    /// Receive changes from the model and send the Send Requests to the View
    /// </summary>
    class ChatWindowViewModele : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ChatWindowModele chatWindowModele = new ChatWindowModele();

        public RelayCommand<object> SendMessageCommand { get; private set; }

        //Contain the information of all message sent in the chatroom
        public ObservableCollection<TextMessage> Items { get; set; }

        //Atribute defining the string message send by a user in the chat
        public string PendingMessageText
        {
            get { return chatWindowModele.PublicMessageText; }
            set
            {
                chatWindowModele.PublicMessageText = value;
                PropertyModified();
            }
        }

        //Constructor
        public ChatWindowViewModele()
        {
            Items = new ObservableCollection<TextMessage>();
            //Sending a message 
            SendMessageCommand = new RelayCommand<object>(SendMessage);
        }

        public void SendMessage(object o)
        {
            //Create a new collection when it's empty
            if (Items == null)
            {
                new ObservableCollection<TextMessage>();
            }

            //TODO: Connect the message with the server
            //SocketHandler HardCodedSocket = new SocketHandler("www.goatsupport.net");
            //Messenger messageSended = new Messenger(HardCodedSocket);
            //messageSended.SendChatMessage(PendingMessageText);

            //Sending all the information about the item
            if (PendingMessageText != string.Empty)
            { 
            Items.Add(new TextMessage
            {
                Title = PendingMessageText,
                MessageSentTime = DateTime.UtcNow,
                SentByMe = true,
                SenderName = "Knuckle Da Weychidna",
                NewItem = true,
            });
            }
            //clear message after it's transmission
            PendingMessageText = string.Empty;
        }


        /// <summary>
        /// Call when a property of the ViewModel is changed
        /// An event is send by the Viewmodel then
        /// The event contains the name of the property modified and will be catched by the View
        /// The View will than update the componant concerned
        /// </summary>
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //Temporary fake text message
    class TextMessage
    {
        public string Title { get; set; }
        public System.DateTime MessageSentTime { get; set; }
        public bool SentByMe { get; set; }
        public string SenderName { get; set; }
        public bool NewItem { get; set; }

        public TextMessage()
        {
            Title = "";
            MessageSentTime = DateTime.UtcNow;
            SentByMe = false;
            SenderName = "missingno";
            NewItem = true;
        }
    }


}