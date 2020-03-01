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
        private static System.Timers.Timer aTimer;
        public ReadyEvent(bool addToAll, DiscordSocketClient discord, ILogger logger) {
            _addToAll = addToAll;
            _discord = discord;
            _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
            _logger = logger;
        }

        private static void SetTimer(double time)
        {
            aTimer = new System.Timers.Timer(time);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var data = await _manager.GetAllUsers();
            data.ForEach(async u =>
            {
                if (u.LastDaily != new TimeSpan(0, 0, 0))
                {
                    await _manager.UpdateUser(u.UserId.ToString(), "LastDaily", (u.LastDaily - TimeSpan.FromMinutes(10).Add(TimeSpan.FromSeconds(15))).ToString());
                }

                if (u.LastRep != new TimeSpan(0, 0, 0))
                {
                    await _manager.UpdateUser(u.UserId.ToString(), "LastRep", (u.LastRep - TimeSpan.FromMinutes(10).Add(TimeSpan.FromSeconds(15))).ToString());
                }

                if (u.LastWork != new TimeSpan(0, 0, 0))
                {
                    await _manager.UpdateUser(u.UserId.ToString(), "LastWork", (u.LastWork - TimeSpan.FromMinutes(10).Add(TimeSpan.FromSeconds(15))).ToString());
                }
            });
        }
    
        public async Task OnReadyEvent()
        {
            SetTimer(615000);
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
