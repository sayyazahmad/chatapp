using ChatApp.Models;

namespace ChatApp
{
    public static class Initializer
    {
        public static List<Team> InitializeTeams()
        {
            var teams = new List<Team>
            {
                new Team("TeamA", new List<Agent> 
                { 
                    new Agent("Lead", 0.5),
                    new Agent("Mid1", 0.6),
                    new Agent("Mid2", 0.6),
                    new Agent("Jnr1", 0.4),
                    new Agent("Jnr2", 0.4)
                }),
                new Team("TeamB", new List<Agent>
                {
                    new Agent("Snr", 0.8),
                    new Agent("Mid1", 0.6),
                    new Agent("Jnr1", 0.4),
                    new Agent("Jnr2", 0.4)
                }),
                new Team("TeamC", new List<Agent>
                {
                    new Agent("Mid1", 0.6),
                    new Agent("Mid2", 0.6)
                }),
                new Team("Overflow", new List<Agent>
                {
                    new Agent("Jnr1", 0.4),
                    new Agent("Jnr2", 0.4),
                    new Agent("Jnr3", 0.4),
                    new Agent("Jnr4", 0.4),
                    new Agent("Jnr5", 0.4),
                    new Agent("Jnr6", 0.4)
                })
            };
            return teams;
        }
    }
}
