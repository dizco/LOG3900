using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.ViewModels
{
    /// <summary>
    ///     Expose the commands and properties connected with the model to the 
    ///     elements of the HistoryWindowView.
    ///     Receive changes from the model and send the Send Requests to the View
    /// </summary>
    internal class HistoryWindowViewModel : ViewModelBase , INotifyPropertyChanged
    {
        //Constructor
        public HistoryWindowViewModel()
        {
            HistoryChanges = new ObservableCollection<HistoryChangeDisplayModel>(); ;
        }

        private string _pageIndex = "1 / 1";

        public string PageIndex
        {
            get => _pageIndex;
            set
            {
                _pageIndex = value;
                PropertyModified();
            }
        }

        //Contain the information of all changes occuring in the editor
        public ObservableCollection<HistoryChangeDisplayModel> HistoryChanges { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

//        TODO : sync HistoryChanges to the changes on the drawing
//        To add in the collection :
//        HistoryChanges.Add(new HistoryChangeDisplayModel
//        {
//            ChangeText = message,
//            Timestamp = messageTime,
//            AuthorName = author
//        });

        /// <summary>
        ///     Called when a property of the ViewModel is changed.
        ///     An event is sent by the Viewmodel then
        ///     The event contains the name of the property modified. The event will be
        ///     catched by the View and the View will then update the component concerned
        /// </summary>
        protected virtual void PropertyModified([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }
    }
}
