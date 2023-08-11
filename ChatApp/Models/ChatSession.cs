namespace ChatApp.Models
{
    public class ChatSession
    {
        public int SessionId { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
