using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Tippy.Database;
using Tippy.Events;
using Tippy.Database.Models;

namespace Tippy.Services
{
    public class LoggingService
    {
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private static Manager _manager;
        public PictureService PictureService = new PictureService();
        public LoggingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<ILogger<LoggingService>>();
            _discord.Ready += new ReadyEvent(true, _discord, _logger).OnReadyEvent;
            _discord.Log += OnLogAsync;
            _discord.JoinedGuild += OnJoinedGuild;
            _discord.LeftGuild += OnLeftGuild;
            _discord.MessageReceived += new MessageEvent(_discord, _logger).OnMessageAsync;
            _discord.UserLeft += new UserLeftEvent(_discord, _logger, PictureService).OnUserLeftEvent;
            _discord.UserJoined += new UserJoinEvent(_discord, _logger, PictureService).OnUserJoinEvent;
            _commands.Log += OnLogAsync;
            _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
        }

        public async Task OnLeftGuild(SocketGuild guild)
        {
            _logger.LogInformation($"Kicked from {guild.Name}, Deleting database...");
            await _manager.DeleteGuildById(guild.Id.ToString());
        }
        public async Task OnJoinedGuild(SocketGuild guild)
        {
            _logger.LogInformation($"Invited to {guild.Name}, Adding database...");
            var newData = new Guild
            {
                GuildId = guild.Id.ToString(),
                Prefix = "s.",
                WelcomeChannel = "",
                LeaveChannel = "",
                EnabledLeave = true,
                EnabledWelcome = true,
            };

            await _manager.InsertGuild(newData);
        }
        public Task OnLogAsync(LogMessage msg)
        {
            string logText = $": {msg.Exception?.ToString() ?? msg.Message}";
            switch (msg.Severity.ToString())
            {
                case "Critical":
                    {
                        _logger.LogCritical(logText);
                        break;
                    }
                case "Warning":
                    {
                        _logger.LogWarning(logText);
                        break;
                    }
                case "Info":
                    {
                        _logger.LogInformation(logText);
                        break;
                    }
                case "Verbose":
                    {
                        _logger.LogInformation(logText);
                        break;
                    }
                case "Debug":
                    {
                        _logger.LogDebug(logText);
                        break;
                    }
                case "Error":
                    {
                        _logger.LogError(logText);
                        break;
                    }
            }

            return Task.CompletedTask;

        }
    }
}