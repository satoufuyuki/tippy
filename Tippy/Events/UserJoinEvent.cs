using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tippy.Database;

namespace Tippy.Events
{
    public class UserJoinEvent
    {
        private readonly DiscordSocketClient _discord;
        private ILogger _logger;
        private Manager _manager;
        private PictureService _pictureService;
        public UserJoinEvent(DiscordSocketClient discord, ILogger logger, PictureService pictureService)
        {
            _discord = discord;
            _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
            _logger = logger;
            _pictureService = pictureService;
        }

        public async Task<Task> OnUserJoinEvent(SocketGuildUser user)
        {
            var data = await _manager.GetGuildByField("GuildId", user.Guild.Id.ToString());
            if (data.FirstOrDefault() == null)
            {
                return Task.CompletedTask;
            }

            var guild = data.FirstOrDefault();
            if (guild.EnabledWelcome == true)
            {
                ulong channelId = (ulong)Convert.ToInt64(guild.WelcomeChannel);
                var channel = user.Guild.GetTextChannel(channelId);
                if (channel == null) return Task.CompletedTask;
                var stream = await _pictureService.GetWelcomeCanvasAsync(user.GetAvatarUrl(Discord.ImageFormat.Png, 2048), $"{user.Username}#{user.Discriminator}", $"This server now has {user.Guild.MemberCount} members");
                stream.Seek(0, SeekOrigin.Begin);
                await channel.SendFileAsync(stream, "welcome.png", $"Welcome to **{user.Guild.Name}**, **{user.Username}#{user.Discriminator}**");
            }
            return Task.CompletedTask;
        }
    }
}
