using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tippy.Database;
using Tippy.Services;

namespace Tippy.Events
{
    public class MessageEvent
    {
        private readonly DiscordSocketClient _discord;
        private ILogger _logger;
        private LevelingUtil LevelingUtil;
        private Manager _manager;
        private static readonly Random rand = new Random();
        private Color GetRandomColour()
        {
            return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
        }
        public MessageEvent(DiscordSocketClient discord, ILogger logger)
        {
            _logger = logger;
            _discord = discord;
            LevelingUtil = new LevelingUtil();
            _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
        }

        private async void WelcomeBack(SocketUserMessage message, string id)
        {
            var rawData = await _manager.GetUsersByField("UserId", id);
            var data = rawData.FirstOrDefault();
            if (data.AfkAttachment != "")
            {
                await _manager.UpdateUser(id, "AfkAttachment", "");
            }

            await _manager.UpdateUser(id, "IsAfk", "false");
            await _manager.UpdateUser(id, "AfkReason", "");
            await _manager.UpdateUser(id, "AfkTime", "");
            var m = await message.Channel.SendMessageAsync($"Welcome back, {message.Author.ToString()} ! I've removed your AFK.");
            await Task.Delay(10000).ContinueWith(async _ =>
             {
                 await m.DeleteAsync();
                 return Task.CompletedTask;
             });
        }

        private async void NotifyAfk(SocketUserMessage message, string id, SocketUser user)
        {
            var rawData = await _manager.GetUsersByField("UserId", id);
            var data = rawData.FirstOrDefault();
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor($"{user.ToString()} is now AFK", user.GetAvatarUrl(ImageFormat.Png, 2048));
            builder.WithColor(GetRandomColour());
            builder.WithDescription($"AFK Since : `{data.AfkTime}`");
            builder.AddField("Reason", data.AfkReason);
            if (data.AfkAttachment != "")
            {
                builder.WithImageUrl(data.AfkAttachment);
            }
            await message.Channel.SendMessageAsync(embed: builder.Build());
        }

        public async Task OnMessageAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            var users = message.MentionedUsers;
            var raw = await _manager.GetUsersByField("UserId", message.Author.Id.ToString());
            if (raw.FirstOrDefault() == null)
            {
                var data = new User
                {
                    UserId = message.Author.Id,
                    Rep = 0,
                    Xp = 0,
                    Level = 0,
                    Money = 0,
                    LastDaily = new TimeSpan(0, 0, 0),
                    NextDaily = new TimeSpan(0, 0, 0),
                    LastRep = new TimeSpan(0, 0, 0),
                    NextRep = new TimeSpan(0, 0, 0),
                    LastWork = new TimeSpan(0, 0, 0),
                    NextWork = new TimeSpan(0, 0, 0),
                    IsAfk = false,
                    AfkReason = "",
                    AfkAttachment = "",
                    AfkTime = ""
                };
                await _manager.InsertUser(data);
            }

            if (raw.FirstOrDefault() != null && raw.FirstOrDefault().IsAfk == true)
            {
                WelcomeBack(message, message.Author.Id.ToString());
            }

            if (users.Count > 0)
            {
                foreach (var user in users.ToArray())
                {
                    var data = await _manager.GetUsersByField("UserId", user.Id.ToString());
                    if (data.FirstOrDefault().IsAfk == true)
                    {
                        NotifyAfk(message, user.Id.ToString(), user);
                    }
                }
            }

            if (!LevelingUtil.isUserHasSpamFilter((long)message.Author.Id))
            {
                var user = await _manager.GetUsersByField("UserId", message.Author.Id.ToString());
                var userprof = user.FirstOrDefault();
                int currentLevel = userprof.Level;
                int currentXp = userprof.Xp;
                int randomXp = (int)LevelingUtil.randomXp(10, 25);
                int newXp = userprof.Xp + randomXp;
                int newLevel = LevelingUtil.xpToLevels(newXp);

                if (newLevel > currentLevel)
                {
                    await _manager.UpdateUser(message.Author.Id.ToString(), "Xp", newXp.ToString());
                    await _manager.UpdateUser(message.Author.Id.ToString(), "Level", newLevel.ToString());
                    var msg = await message.Channel.SendMessageAsync($"🎉 Congratulations <@{message.Author.Id}> ! You has leveled up to **`{newLevel}`** !");
                } else
                {
                    await _manager.UpdateUser(message.Author.Id.ToString(), "Xp", newXp.ToString());
                }

                LevelingUtil.addUserToSpamFilter((long)message.Author.Id);
            }
        }
    }
}