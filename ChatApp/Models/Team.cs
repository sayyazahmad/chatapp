namespace ChatApp.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<Agent> Agents { get; set; }
        public int Capacity { get; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsOverflow { get; set; }

        public Team(string name, TimeSpan startTime, TimeSpan endTime, List<Agent> agents, bool isOverflow = false)
        {
            this.Name = name;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Agents = agents;
            this.Capacity = agents.Sum(a => a.Capacity);
            IsOverflow = isOverflow;
        }
    }
}
