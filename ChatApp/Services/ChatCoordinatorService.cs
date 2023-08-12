using ChatApp.Models;

namespace ChatApp.Services
{
    public class ChatCoordinatorService : IChatCoordinatorService
    {
        private readonly Queue<ChatSession> chatQueue;
        private readonly List<Team> teams;
        private readonly ILogger<ChatCoordinatorService> logger;

        public ChatCoordinatorService(Queue<ChatSession> chatQueue, List<Team> teams, ILogger<ChatCoordinatorService> logger)
        {
            this.chatQueue = chatQueue;
            this.teams = teams;
            this.logger = logger;
        }

        public string? CreateChatSession(ChatSession chatSession)
        {
            ArgumentNullException.ThrowIfNull(chatSession);

            chatSession.SessionId = GenerateSessionId();
            chatQueue.Enqueue(chatSession);

            return "Ok";
        }

        public string? AssignChatToAgent()
        {
            if (chatQueue.Count > 0)
            {
                var team = teams.Find(x => DateTime.UtcNow.TimeOfDay >= x.StartTime && DateTime.UtcNow.TimeOfDay < x.EndTime);
                if (team?.Agents?.Any() ?? false)
                {
                    foreach (var agentGroup in team.Agents.OrderBy(x => x.Efficiency).GroupBy(x => x.Efficiency).Select(grp => grp.ToList()).ToList())
                    {
                        for (int i = 0; i < agentGroup.Count; i++)
                        {
                            if (agentGroup[i].Load < agentGroup[i].Capacity)
                            {
                                agentGroup[i].Chats.Add(chatQueue.Dequeue());
                                return agentGroup[i].Name;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private int GenerateSessionId()
        {
            var now = DateTime.Now;
            var zeroDate = DateTime.MinValue.AddHours(now.Hour).AddMinutes(now.Minute).AddSeconds(now.Second).AddMilliseconds(now.Millisecond);
            return (int)(zeroDate.Ticks / 10000);
        }
    }
}
