namespace ChatClient.Client
{
    public interface IClientMethods
    {
        void Message(MessageParam param);
        void ClientList(ClientListParam param);
    }
}