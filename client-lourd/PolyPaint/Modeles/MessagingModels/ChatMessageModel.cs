namespace PolyPaint.Modeles.MessagingModels
{
    public class ChatMessageModel : MessageModelBase
    {
        public string message { get; set; }
        public AuthorModel author { get; set; }
        public RoomModel room { get; set; }
        public int timestamp { get; set; } = 0;
    }
}