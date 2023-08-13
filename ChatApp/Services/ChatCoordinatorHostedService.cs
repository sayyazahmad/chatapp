namespace ChatApp.Services
{
    /// <summary>
    /// Background job that pools every second for any new chat sessions, if available the service 
    /// then assigns chat to next available agent in round robin fashion.
    /// </summary>
    public class ChatCoordinatorHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<ChatCoordinatorHostedService> logger;
        Timer? timer = null;
        private readonly IServiceProvider serviceProvider;

        public ChatCoordinatorHostedService(ILogger<ChatCoordinatorHostedService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Chat Coordinator Service running.");

            timer = new Timer(ProcessChat, null, TimeSpan.Zero, TimeSpan.FromSeconds(100));

            return Task.CompletedTask;
        }

        private void ProcessChat(object? state)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IChatService chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
            var agent = chatService.AssignChatToAgent();

            logger.LogInformation($"Chat assigned to {agent} at:{DateTime.UtcNow}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Chat Coordinator Service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose() => timer?.Dispose();
    }
}
