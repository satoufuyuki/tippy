using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        public MessageEvent(DiscordSocketClient discord, ILogger logger)
        {
            _logger = logger;
            _discord = discord;
            LevelingUtil = new LevelingUtil();
            _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
        }

        public async Task OnMessageAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            if (!LevelingUtil.isUserHasSpamFilter((long)message.Author.Id))
            {
                var user = await _manager.GetUsersByField("UserId", message.Author.Id.ToString());
                if (user.Count == 0)
                {
                    var data = new Database.User
                    {
                        UserId = message.Author.Id,
                        Xp = 0,
                        Level = 0,
                        Money = 0
                    };
                    await _manager.InsertUser(data);
                    return;
                }
                
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
                /*
                Console.WriteLine($"== {message.Author.Username} ==");
                Console.WriteLine($"Converted xp : {Convert.ToInt32(userprof.Xp)} \nType : {currentXp.GetType()}");
                Console.WriteLine($"Random XP : {randomXp}");
                Console.WriteLine($"Current Level : {currentLevel}");
                Console.WriteLine($"New Level : {newlevel}");
                Console.WriteLine($"New Xp : {newXp}");

                if (newlevel > currentLevel)
                {
                    _handler.AddLevel(message.Author.Id, 1);
                    _handler.AddXp(message.Author.Id, newXp);
                    var msg = await message.Channel.SendMessageAsync($"🎉 Congratulations <@{message.Author.Id}> ! You has leveled up to **`{newlevel}`** !");
                }
                else
                {
                    _handler.AddXp(message.Author.Id, newXp);
                }
                //LevelingUtil.addUserToSpamFilter((long)message.Author.Id);*/
            }
        }
    }
}