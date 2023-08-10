using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IChatCoordinatorService
    {
        string? CreateChatSession(ChatSession chatSession);
        string? AssignChatToAgent();
    }
}