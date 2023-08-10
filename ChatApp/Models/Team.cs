namespace ChatApp.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<Agent> Agents { get; set; }
        public int Capacity { get; }

        public Team(string name, List<Agent> agents)
        {
            this.Name = name;
            this.Agents = agents;
            this.Capacity = agents.Sum(a => a.Capacity);
        }
    }
}
