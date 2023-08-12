using ChatApp;
using ChatApp.Models;
using ChatApp.Services;
using ChatApp.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<OfficeHoursSettings>(builder.Configuration.GetSection("OfficeHours"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<List<Team>>(Initializer.InitializeTeams());
builder.Services.AddSingleton<Queue<ChatSession>>();
builder.Services.AddScoped<IChatService, ChatService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
