using Chat.Data;
using Chat.Hubs;
using Chat.Interfaces;
using Chat.Models;
using Chat.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options =>
    options.Connect(builder.Configuration["ConnectionStrings:AzureAppConfig"]));

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("SignalConnection");
builder.Services.AddSignalR().AddAzureSignalR(connectionString);
builder.Services.Configure<AzureTextAnalyticsSettings>(builder.Configuration.GetSection("AzureTextAnalytics"));

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ChatDatabase")));

// Register my custom services
builder.Services.AddScoped<IChatConnectionService, ChatConnectionService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<ITextAnalyticsService, TextAnalyticsService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();

app.MapHub<ChatHub>("/chat");

app.Run();

