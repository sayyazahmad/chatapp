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
        private readonly IChatService chatService;
        private readonly OfficeHoursSettings officeHours;
        private readonly ILogger<ChatController> logger;

        public ChatController(Queue<ChatSession> chatQueue, 
            List<Team> teams,
            IChatService chatService,
            IOptions<OfficeHoursSettings> options,
            ILogger<ChatController> logger)
        {
            this.chatQueue = chatQueue;
            this.teams = teams;
            this.chatService = chatService;
            this.officeHours = options.Value;
            this.logger = logger;
        }

        /// <summary>
        /// API endpoint to register new chat session
        /// </summary>
        /// <param name="chatSession">New chat session object from client</param>
        /// <returns>Endpoint registers chat session into chat queue and returns Ok. If the queue if full No available agent message is returned.</returns>
        [HttpPost("create")]
        public IActionResult CreateChatSession(ChatSession chatSession)
        {
            ArgumentNullException.ThrowIfNull(chatSession);

            try
            {
                //team working in current shift
                var team = teams.Find(x => DateTime.UtcNow.TimeOfDay >= x.StartTime && DateTime.UtcNow.TimeOfDay < x.EndTime);
                //office hours
                if (team?.StartTime >= officeHours.Start && team?.EndTime < officeHours.End)
                {
                    if (chatQueue.Count >= team.Capacity)
                    {
                        var overflowTeam = teams.Where(x => x.IsOverflow)?.FirstOrDefault();
                        if (overflowTeam is not null)
                        {
                            var res = chatService.CreateChatSession(chatSession);
                            return Ok("Ok");
                        }
                        else return BadRequest("No agent available");
                    }
                    else
                    {
                        var res = chatService.CreateChatSession(chatSession);
                        return Ok("Ok");
                    }
                }
                else
                {
                    if (chatQueue?.Count >= team?.Capacity)
                    {
                        return BadRequest("No agent available");
                    }
                    else
                    {
                        var res = chatService.CreateChatSession(chatSession);
                        return Ok("Ok");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError($"Error executing {nameof(CreateChatSession)}: {ex.Message}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// API endpoint to trigger chat session assignment service
        /// Assignment service then assigns chat to next available agent in round robin fashion.
        /// </summary>
        /// <returns>If chat assigned successfully, the API returns team name -> agent name, returns 400-BadRequest instead </returns>
        [HttpPost("assign")]
        public IActionResult AssignChatToAgent()
        {
            try
            {
                if (chatQueue.Count > 0)
                {
                    var res = chatService.AssignChatToAgent();
                    if (!string.IsNullOrEmpty(res))
                        return Ok(res);
                    else
                        return BadRequest("Agent not available.");
                }
                else
                    return BadRequest("No chat sessions in the queue.");

            }
            catch (Exception ex)
            {
                logger.LogError($"Error executing {nameof(CreateChatSession)}: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
