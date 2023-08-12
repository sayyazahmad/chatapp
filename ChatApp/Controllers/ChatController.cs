﻿using ChatApp.Models;
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
        private readonly ILogger<ChatController> logger;

        public ChatController(Queue<ChatSession> chatQueue, 
            List<Team> teams,
            IChatCoordinatorService chatCoordinatorService,
            IOptions<OfficeHoursSettings> options,
            ILogger<ChatController> logger)
        {
            this.chatQueue = chatQueue;
            this.teams = teams;
            this.chatCoorinatorService = chatCoordinatorService;
            this.officeHours = options.Value;
            this.logger = logger;
        }

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
                            var res = chatCoorinatorService.CreateChatSession(chatSession);
                            return Ok("Ok");
                        }
                        else return BadRequest("Queue Full");
                    }
                    else
                    {
                        var res = chatCoorinatorService.CreateChatSession(chatSession);
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
                        var res = chatCoorinatorService.CreateChatSession(chatSession);
                        return Ok("Ok");
                    }
                }

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("assign")]
        public IActionResult AssignChatToAgent()
        {
            try
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
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
