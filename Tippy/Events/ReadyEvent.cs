using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Tippy.Database;
using Tippy.Database.Models;

namespace Tippy.Events
{
    public class ReadyEvent
    {
        private bool _addToAll;
        private readonly DiscordSocketClient _discord;
        private ILogger _logger;
        private static Manager _manager;
        public ReadyEvent(bool addToAll, DiscordSocketClient discord, ILogger logger) {
            _addToAll = addToAll;
            _discord = discord;
            _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
            _logger = logger;
        }

        public async Task OnReadyEvent()
        {
            if (_addToAll)
            {
                var guilds = _discord.Guilds;
                foreach (var guild in guilds)
                {
                    var data = await _manager.GetGuildByField("GuildId", guild.Id.ToString());
                    if (data.FirstOrDefault() == null)
                    {
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
                        _logger.LogInformation($"{guild.Name} doesn't exists in my database. This server have been added into database.");
                    }
                }
            }

            _logger.LogInformation($"Connected as -> {_discord.CurrentUser.Username}#{_discord.CurrentUser.DiscriminatorValue} :)");
            _logger.LogInformation($"We are on {_discord.Guilds.Count} servers");
        }
    }
}
