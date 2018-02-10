namespace PolyPaint.Helpers.Communication
{
    public interface ISocketHandler
    {
        bool IsConnected { get; }

        bool SendMessage(string data);

        void ConnectSocket();
        
        void DisconnectSocket();
    }
}