using ChatApp.Models;

namespace ChatApp.Services
{
    public class ChatCoordinatorService : IChatCoordinatorService
    {
        private readonly Queue<ChatSession> chatQueue;
        private readonly List<Team> teams;
        int position = 0;

        public ChatCoordinatorService(Queue<ChatSession> chatQueue, List<Team> teams)
        {
            this.chatQueue = chatQueue;
            this.teams = teams;
        }

        public string? CreateChatSession(ChatSession chatSession)
        {
            ArgumentNullException.ThrowIfNull(chatSession);

            chatSession.SessionId = GenerateSessionId();
            chatQueue.Enqueue(chatSession);

            // Find the next available agent to assign the chat request to.
            //Agent agent = await this.FindNextAvailableAgent();

            //// Assign the chat request to the agent.
            //agent.Chats.Add(chatRequest);

            //// Notify the agent that they have a new chat request.
            //agent.Notify();

            return "Ok";
        }

        public string? AssignChatToAgent()
        {
            if (chatQueue.Count > 0)
            {
                var chatSession = chatQueue.Dequeue();

                var availableAgent = FindNextAvailableAgent();

                if (availableAgent is not null)
                {
                    // Assign chat session to available agent
                    availableAgent.Chats.Add(chatSession);
                    availableAgent.Capacity --;
                    return availableAgent.Name;
                }
                else return null;
            }
            else return null;
        }

        private Agent FindNextAvailableAgent()
        {
            var currentTeam = teams.Find(x => DateTime.UtcNow.TimeOfDay >= x.StartTime && DateTime.UtcNow.TimeOfDay < x.EndTime);
            var agents = currentTeam?.Agents?.OrderByDescending(x => x.Efficiency).ToList();
            if (agents.Any())
            {

                int next = position % agents.Count();
                while (agents[next].Efficiency < 0.4)
                {
                    next = (next + 1) % agents.Count;
                }

                position++;
                return agents[next];
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
