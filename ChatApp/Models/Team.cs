namespace ChatApp.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<Agent> Agents { get; set; }
        public int Capacity => Convert.ToInt32(Agents.Sum(a => a.Capacity) * 1.5);
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsOverflow { get; set; }
        public int? Load => Agents?.Sum(x => x.Load);
        public Team(string name, TimeSpan startTime, TimeSpan endTime, List<Agent> agents, bool isOverflow = false)
        {
            this.Name = name;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Agents = agents;
            IsOverflow = isOverflow;
        }
    }
}
