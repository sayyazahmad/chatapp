namespace ChatApp.Services
{
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
            logger.LogInformation("Timed Hosted Service running.");

            timer = new Timer(ProcessChat, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        private void ProcessChat(object? state)
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IChatService chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

                chatService.AssignChatToAgent();
            }
            logger.LogInformation($"Chat Proccessed");
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
