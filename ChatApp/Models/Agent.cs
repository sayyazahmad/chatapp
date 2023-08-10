namespace ChatApp.Models
{
    public class Agent
    {
        public string Name { get; set; }
        public double Efficiency { get; set; }
        public int Capacity { get; set; }
        public List<ChatSession> Chats { get; set; }
        public bool IsAvailable { get; set; }

        public Agent(string name, double efficiency)
        {
            this.Name = name;
            this.Efficiency = efficiency;
            this.Capacity = (int)(10 * efficiency);
            this.Chats = new List<ChatSession>();
            this.IsAvailable = true;
        }

        public void MarkUnavailable()
        {
            this.IsAvailable = false;
        }
    }

    public enum SeniorityLevel
    {
        Junior,
        MidLevel,
        Senior,
        TeamLead
    }
}
