using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tippy.Database;

namespace Tippy.Events
{
    public class UserLeftEvent
    {
        private readonly DiscordSocketClient _discord;
        private ILogger _logger;
        private Manager _manager;
        private PictureService _pictureService;
        public UserLeftEvent(DiscordSocketClient discord, ILogger logger, PictureService pictureService)
        {
            _discord = discord;
            _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
            _logger = logger;
            _pictureService = pictureService;
        }

        public async Task<Task> OnUserLeftEvent(SocketGuildUser user)
        {
            var data = await _manager.GetGuildByField("GuildId", user.Guild.Id.ToString());
            if (data.FirstOrDefault() == null)
            {
                return Task.CompletedTask;
            }

            var guild = data.FirstOrDefault();
            if (guild.EnabledLeave == true)
            {
                ulong channelId = (ulong)Convert.ToInt64(guild.LeaveChannel);
                var channel = user.Guild.GetTextChannel(channelId);
                if (channel == null) return Task.CompletedTask;
                var stream = await _pictureService.GetLeaveCanvasAsync(user.GetAvatarUrl(Discord.ImageFormat.Png, 2048), $"{user.Username}#{user.Discriminator}", $"This server now has {user.Guild.MemberCount} members");
                stream.Seek(0, SeekOrigin.Begin);
                await channel.SendFileAsync(stream, "leave.png", $"Goodbye, **{user.Username}#{user.Discriminator}**");
            }
            return Task.CompletedTask;
        }
    }
}
