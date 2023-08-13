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
        /// API endpointe to register new chat session
        /// </summary>
        /// <param name="chatSession">New chat session object from client</param>
        /// <returns>Retuns ok if queue if available</returns>
        [HttpPost("create")]
        public IActionResult CreateChatSession(ChatSession chatSession)
        {
            ArgumentNullException.ThrowIfNull(chatSession);

            try
            {
                var currentTeam = teams.Find(x => DateTime.UtcNow.TimeOfDay >= x.StartTime && DateTime.UtcNow.TimeOfDay < x.EndTime);
                //office hours
                if (currentTeam?.StartTime >= officeHours.Start && currentTeam?.EndTime < officeHours.End)
                {
                    if (chatQueue.Count >= currentTeam.Capacity)
                    {
                        var overflowTeam = teams.Where(x => x.IsOverflow)?.FirstOrDefault();
                        if (overflowTeam is not null)
                        {
                            var res = chatService.CreateChatSession(chatSession);
                            return Ok("Ok");
                        }
                        else return BadRequest("Queue Full");
                    }
                    else
                    {
                        var res = chatService.CreateChatSession(chatSession);
                        return Ok("Ok");
                    }
                }
                else
                {
                    if (chatQueue?.Count >= currentTeam?.Capacity)
                    {
                        return BadRequest("Queue Full");
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
        /// </summary>
        /// <returns></returns>
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
                    return BadRequest("No chat sessions in the queue.");
            }
            else
                return BadRequest("No chat sessions in the queue.");

            }
            catch (Exception)
            {
                logger.LogError($"Error executing {nameof(CreateChatSession)}: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
