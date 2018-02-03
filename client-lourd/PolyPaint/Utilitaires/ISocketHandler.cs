namespace PolyPaint.Utilitaires
{
    public interface ISocketHandler
    {
        bool IsConnected { get; }

        bool SendMessage(string data);

        void ConnectSocket();
        
        void DisconnectSocket();
    }
}