namespace SignalrChatApp.ViewModels
{
    public class MessagesVM
    {
        public Guid MessageId { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public string MessageGroupId { get; set; }
        public string SenderUser { get; set; }
        public string ReceiverUser { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
