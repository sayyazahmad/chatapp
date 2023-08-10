using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly Queue<ChatSession> chatQueue = new Queue<ChatSession>();
        private readonly List<Team> teams = new List<Team>();
        private readonly IChatCoordinatorService chatCoorinatorService;

        public ChatController(Queue<ChatSession> chatQueue, List<Team> teams, IChatCoordinatorService chatCoordinatorService)
        {
            this.chatQueue = chatQueue;
            this.teams = teams;
            this.chatCoorinatorService = chatCoordinatorService;
        }

        [HttpPost("create")]
        public IActionResult CreateChatSession(ChatSession chatSession)
        {
            ArgumentNullException.ThrowIfNull(chatSession);

            var res = chatCoorinatorService.CreateChatSession(chatSession);
            if (!string.IsNullOrEmpty(res))
            {
                return Ok("Ok");
            }
            else 
                return Ok("NoOk");
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
