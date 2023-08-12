using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IChatService
    {
        string? CreateChatSession(ChatSession chatSession);
        string? AssignChatToAgent();
    }
}