using ChatApp.Models;
using ChatApp.Services;
using ChatApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChatApp.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly Queue<ChatSession> chatQueue;
        private readonly List<Team> teams;
        private readonly IChatCoordinatorService chatCoorinatorService;
        private readonly OfficeHoursSettings officeHours;

        public ChatController(Queue<ChatSession> chatQueue, 
            List<Team> teams, 
            IChatCoordinatorService chatCoordinatorService, 
            IOptions<OfficeHoursSettings> options)
        {
            this.chatQueue = chatQueue;
            this.teams = teams;
            this.chatCoorinatorService = chatCoordinatorService;
            this.officeHours = options.Value;
        }

        [HttpPost("create")]
        public IActionResult CreateChatSession(ChatSession chatSession)
        {
            ArgumentNullException.ThrowIfNull(chatSession);

            //check if it's not office hour
            if (DateTime.UtcNow.TimeOfDay >= officeHours.Start && DateTime.UtcNow.TimeOfDay < officeHours.End)
            {
                var res = chatCoorinatorService.CreateChatSession(chatSession);
                return Ok("Ok");
            }
            else
            {
                //Check the capacity of current team and overflow team
                var currentTeam = teams.Find(x => DateTime.UtcNow.TimeOfDay >= x.StartTime && DateTime.UtcNow.TimeOfDay < x.EndTime);
                var overflowTeam = teams.Where(x => x.IsOverflow)?.FirstOrDefault();

                if (chatQueue?.Count + 1 < ((currentTeam?.Capacity + overflowTeam?.Capacity) * 1.5))
                {
                    return BadRequest("Queue Full");
                }
                else
                {
                    var res = chatCoorinatorService.CreateChatSession(chatSession);
                    return Ok("Ok");
                }
            }
        }

        [HttpPost("assign")]
        public IActionResult AssignChatToAgent()
        {
            if (chatQueue.Count > 0)
            {
                var res = chatCoorinatorService.AssignChatToAgent();
                if (!string.IsNullOrEmpty(res))
                    return Ok(res);
                else
                    return BadRequest("No chat sessions in the queue.");
            }
            else
                return BadRequest("No chat sessions in the queue.");
        }
    }
}
