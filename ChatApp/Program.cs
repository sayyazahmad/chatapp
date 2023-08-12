using ChatApp;
using ChatApp.Models;
using ChatApp.Services;
using ChatApp.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OfficeHoursSettings>(builder.Configuration.GetSection("OfficeHours"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<List<Team>>(Initializer.InitializeTeams());
builder.Services.AddSingleton<Queue<ChatSession>>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHostedService<ChatCoordinatorHostedService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
