using ChatApp.Models;

namespace ChatApp.Services
{
    /// <summary>
    /// Implementation of IChatService.
    /// 
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly Queue<ChatSession> chatQueue;
        private readonly List<Team> teams;
        private readonly ILogger<ChatService> logger;

        public ChatService(Queue<ChatSession> chatQueue, List<Team> teams, ILogger<ChatService> logger)
        {
            this.chatQueue = chatQueue;
            this.teams = teams;
            this.logger = logger;
        }

        public string? CreateChatSession(ChatSession chatSession)
        {
            ArgumentNullException.ThrowIfNull(chatSession);

            chatSession.SessionId = GenerateSessionId();
            chatSession.CreatedOn = DateTime.UtcNow;
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
                    //Break teams into subgroups based on agent efficiency. Traverse all the groups starting from lower efficiencey.
                    foreach (var agentGroup in team.Agents.OrderBy(x => x.Efficiency).GroupBy(x => x.Efficiency).Select(grp => grp.ToList()).ToList())
                    {
                        for (int i = 0; i < agentGroup.Count; i++)
                        {
                            if (agentGroup[i].Load < agentGroup[i].Capacity)
                            {
                                //Chat session assignment agent
                                agentGroup[i].Chats.Add(chatQueue.Dequeue());
                                // *** TODO ***
                                //Send chat assignment notification to agent
                                return $"{team.Name}->{agentGroup[i].Name}";
                            }
                        }
                    }
                }
            }
            return null;
        }

        public bool EndChatSession(Agent agent, ChatSession chat)
        {
            ArgumentNullException.ThrowIfNull(agent);
            ArgumentNullException.ThrowIfNull(chat);
            return agent.Chats.Remove(chat);
        }

        private int GenerateSessionId()
        {
            var now = DateTime.Now;
            var zeroDate = DateTime.MinValue.AddHours(now.Hour).AddMinutes(now.Minute).AddSeconds(now.Second).AddMilliseconds(now.Millisecond);
            return (int)(zeroDate.Ticks / 10000);
        }
    }
}
