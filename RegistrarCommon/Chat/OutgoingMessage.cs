namespace RegistrarCommon.Chat
{
    public class OutgoingMessage
    {
        public string SenderUserID { get; set; }
        public string DestinationUserID { get; set; }
        public string Text { get; set; }
    }
}