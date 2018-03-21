using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PolyPaint.Helpers;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.ViewModels
{
    /// <summary>
    ///     Expose the commands and properties connected with the model to the elements of the ChatWindowView
    ///     Receive changes from the model and send the Send Requests to the View
    /// </summary>
    internal class ChatViewModel : ViewModelBase, INotifyPropertyChanged
    {
        //Atribute defining the string message send by a user in the chat

        private string _outgoingChatMessage;

        //Constructor
        public ChatViewModel()
        {
            ChatMessages = ChatMessageCollection;

            //Sending a message 
            SendMessageCommand = new RelayCommand<object>(SendMessage);
        }

        public RelayCommand<object> SendMessageCommand { get; }

        //Contain the information of all message sent in the chatroom
        public ObservableCollection<ChatMessageDisplayModel> ChatMessages { get; set; }

        public string OutgoingChatMessage
        {
            get => _outgoingChatMessage;
            set
            {
                _outgoingChatMessage = value;
                PropertyModified();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SendMessage(object o)
        {
            //Sending all the information about the item
            if (!string.IsNullOrWhiteSpace(OutgoingChatMessage) && Messenger.IsConnected)
            {
                Messenger.SendChatMessage(OutgoingChatMessage);
                OutgoingChatMessage = string.Empty;
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
}
