namespace ChatApp.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Efficiency { get; set; }
        public int Capacity { get; set; }
        public List<ChatSession> Chats { get; set; }
        public bool IsAvailable { get; set; }
        public int? Load => Chats?.Count;
        
        public Agent(int id, string name, double efficiency)
        {
            Id = id;
            Name = name;
            Efficiency = efficiency;
            Capacity = (int)(10 * efficiency);
            Chats = new List<ChatSession>();
            IsAvailable = true;
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
