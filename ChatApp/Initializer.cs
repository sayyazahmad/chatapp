using ChatApp.Models;

namespace ChatApp
{
    public static class Initializer
    {
        public static List<Team> InitializeTeams()
        {
            var teams = new List<Team>
            {
                new Team("TeamA", new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0), new List<Agent> 
                { 
                    new Agent(1, "Lead", 0.5),
                    new Agent(2, "Mid1", 0.6),
                    new Agent(3, "Mid2", 0.6),
                    new Agent(4, "Jnr1", 0.4),
                    new Agent(5, "Jnr2", 0.4)
                }),
                new Team("TeamB", new TimeSpan(16, 0, 0), new TimeSpan(22, 0, 0), new List<Agent>
                {
                    new Agent(6, "Snr", 0.8),
                    new Agent(7, "Mid1", 0.6),
                    new Agent(8, "Jnr1", 0.4),
                    new Agent(9, "Jnr2", 0.4)
                }),
                new Team("TeamC", new TimeSpan(22, 0, 0), new TimeSpan(8, 0, 0), new List<Agent>
                {
                    new Agent(10, "Mid1", 0.6),
                    new Agent(11, "Mid2", 0.6)
                }),
                new Team("Overflow", new TimeSpan(8, 0, 0), new TimeSpan(20, 0, 0), new List<Agent>
                {
                    new Agent(12, "Jnr1", 0.4),
                    new Agent(13, "Jnr2", 0.4),
                    new Agent(14, "Jnr3", 0.4),
                    new Agent(15, "Jnr4", 0.4),
                    new Agent(16, "Jnr5", 0.4),
                    new Agent(17, "Jnr6", 0.4)
                }, true)
            };
            return teams;
        }
    }
}
