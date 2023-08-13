using ChatApp.Controllers;
using ChatApp.Models;
using ChatApp.Services;
using ChatApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatApp.Test
{
    /// <summary>
    /// Unit tests of Chat App
    /// </summary>
    public class ChatControllerTest
    {
        private readonly ChatController controller;
        private readonly IChatService chatService;

        private readonly Queue<ChatSession> chatQueue;
        private readonly List<Team> teams;
        private readonly IOptions<OfficeHoursSettings> officeHours;
        private readonly ILogger<ChatService> chatServiceLogger;
        private readonly ILogger<ChatController> chatControllerLogger;

        public ChatControllerTest()
        {
            chatQueue = new Queue<ChatSession>();
            teams = Initializer.InitializeTeams();
            officeHours = Options.Create(new OfficeHoursSettings { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) });
            chatService = new ChatService(chatQueue, teams, chatServiceLogger);
            controller = new ChatController(chatQueue, teams, chatService, officeHours, chatControllerLogger);
        }

        [Fact]
        public void Get_CreateChatSession_Ok()
        {
            var okResult = controller.CreateChatSession(new ChatSession { Message = "Test Chat" });

            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }

        [Fact]
        public void Get_AssignChatToAgent_Ok()
        {
            chatQueue.Enqueue(new ChatSession { Message="Test Chat"});
            var okResult = controller.AssignChatToAgent();

            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }

        [Fact]
        public void Create_Chat_Session_No_Agent_Available()
        {
            for (int i = 0; i < 33; i++)
            {
                chatQueue.Enqueue(new ChatSession { Message = $"Test Chat{i}" });
            }
            var result = controller.CreateChatSession(new ChatSession { Message = "Test Chat" });
            var response = ((BadRequestObjectResult) result).Value;
            Assert.Equal("No agent available", response);
        }
    }
}