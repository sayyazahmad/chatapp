using ChatApp.Models;

namespace ChatApp.Services
{
    /// <summary>
    /// Implementation of IChatService.
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

        /// <summary>
        /// Business logic to register a new chat session
        /// </summary>
        /// <param name="chatSession">New chat session object from client</param>
        /// <returns>Method registers chat session into chat queue and returns Ok</returns>
        public string? CreateChatSession(ChatSession chatSession)
        {
            ArgumentNullException.ThrowIfNull(chatSession);

            chatSession.SessionId = GenerateSessionId();
            chatSession.CreatedOn = DateTime.UtcNow;
            chatQueue.Enqueue(chatSession);

            return "Ok";
        }

        /// <summary>
        /// Business logic to assign chat session to an agent.
        /// Looks for the next available agent and assigns chat session in round robin fashion.
        /// </summary>
        /// <returns>Method returns team name -> agent name</returns>
        public string? AssignChatToAgent()
        {
            if (chatQueue.Count > 0)
            {
                //team working in current shift
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

        /// <summary>
        /// Business logic to end a chat session
        /// Method ends a chat session with agent and make agent available for next session
        /// </summary>
        /// <param name="agent">Agent that holds session</param>
        /// <param name="chat">Chat session that agent hold, and wants to end.</param>
        /// <returns>Returns boolean confirming the end of session</returns>
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
