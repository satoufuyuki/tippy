using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tippy.Database;

namespace Tippy.Modules
{
    [Name("Core")]
    public class CoreModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private static readonly Random rand = new Random();
        private Manager _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
        private Color GetRandomColour()
        {
            return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
        }
        public CoreModule(CommandService service)
        {
            _service = service;
        }

        [Command("ping")]
        [Summary("Ping pong with bot")]
        [Alias("pong")]
        public async Task PingAsync()
        {
            var message = await ReplyAsync("Pinging...");
            var builder = new EmbedBuilder();
            builder.WithAuthor("🏓 Pong !", Context.Message.Author.GetAvatarUrl(ImageFormat.Png, 2048));
            builder.AddField(x =>
            {
                x.Name = "🌐 Websocket Ping";
                x.Value = $"`{Context.Client.Latency}`ms";
                x.IsInline = false;
            });
            builder.AddField(x =>
            {
                x.Name = "📶 Latency Ping";
                x.Value = $"`{(DateTime.Now.Ticks - message.CreatedAt.Ticks).ToString().Substring(0, 3)}`ms";
                x.IsInline = false;
            });
            builder.WithFooter($"• Ping of {Context.Client.CurrentUser.Username}#{Context.Client.CurrentUser.Discriminator}", Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 2048));
            await message.DeleteAsync();
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("help")]
        [Alias("h", "command", "commands", "cmd")]
        [Summary("See my commands list!")]
        public async Task HelpAsync([Remainder] string command = null)
        { 
            var data = await _manager.GetGuildByField("GuildId", Context.Guild.Id.ToString());
            string prefix = data.FirstOrDefault().Prefix;

            if (command == null)
            {
                var builder = new EmbedBuilder()
                {
                    Color = GetRandomColour(),
                    Description = $"Hello, my name is **{Context.Client.CurrentUser.Username}** ! My prefix for this server is `{prefix}`.\n" +
                    $"Use `{prefix}help [command]` to get advanced help."
                };
                builder.WithAuthor($"{Context.Client.CurrentUser.Username}#{Context.Client.CurrentUser.Discriminator} Commands List", Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 2048))
                    .WithCurrentTimestamp()
                    .WithFooter($"• Requested By {Context.Message.Author.Username}#{Context.Message.Author.Discriminator}", Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 2048));

                foreach (var module in _service.Modules)
                {
                    string description = null;
                    foreach (var cmd in module.Commands)
                    {
                        var result = await cmd.CheckPreconditionsAsync(Context);
                        if (result.IsSuccess)
                            description += $" `{cmd.Aliases.First()}` ";
                    }

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        description = description.Replace(@"/\\(\*|_|`​|~|\\)/g", @"$1").Replace(@"/(\*|_|`​|~|\\)/g", @"\\$1");
                        if (module.Name != "Developer")
                        {
                            builder.AddField(x =>
                            {
                                x.Name = module.Name;
                                x.Value = description;
                                x.IsInline = false;
                            });
                        }
                    }
                }

                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                var result = _service.Search(Context, command);

                if (!result.IsSuccess)
                {
                    await ReplyAsync($"Command with name **{command}** doesn't exists.");
                    return;
                }

                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                };

                foreach (var match in result.Commands)
                {
                    var cmd = match.Command;
                    var parameter = string.Join(", ", cmd.Parameters.Select(p => p.Name));
                    if (string.IsNullOrWhiteSpace(parameter))
                    {
                        parameter = "No parameter needed for this command.";
                    }
                    builder.WithAuthor($"{Context.Client.CurrentUser.Username}#{Context.Client.CurrentUser.Discriminator} Advanced Help Command", Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 2048));
                    builder.WithDescription(cmd.Summary);
                    builder.AddField(x =>
                    {
                        x.Name = "Command Aliases";
                        x.Value = string.Join(", ", cmd.Aliases);
                        x.IsInline = false;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "Command Parameter";
                        x.Value = $"{parameter}";
                        x.IsInline = false;
                    })
                        .WithCurrentTimestamp()
                    .WithFooter($"• Requested By {Context.Message.Author.Username}#{Context.Message.Author.Discriminator}", Context.Message.Author.GetAvatarUrl(ImageFormat.Png, 2048));
                }

                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("afk")]
        [Alias("away")]
        [Summary("Set your AFK reason")]
        public async Task AfkAsync(string reason = null)
        {
            if (string.IsNullOrEmpty(reason))
            {
                reason = "None";
            }
            var data = await _manager.GetUsersByField("UserId", Context.Message.Author.Id.ToString());
            if (data.FirstOrDefault().IsAfk == false)
            {
                string now = DateTime.UtcNow.ToString() + " | GMT+0000 (Coordinated Universal Time)";
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithAuthor($"{Context.Message.Author.Username} is now AFK", Context.Message.Author.GetAvatarUrl(ImageFormat.Png, 2048));
                builder.WithColor(GetRandomColour());
                builder.WithDescription($"```\n{reason}\n```");
                if (Context.Message.Attachments.FirstOrDefault() != null)
                {
                    builder.WithImageUrl(Context.Message.Attachments.First().Url);
                    await _manager.UpdateUser(Context.Message.Author.Id.ToString(), "AfkAttachment", Context.Message.Attachments.First().Url);
                }

                await _manager.UpdateUser(Context.Message.Author.Id.ToString(), "IsAfk", "true");
                await _manager.UpdateUser(Context.Message.Author.Id.ToString(), "AfkReason", reason);
                await _manager.UpdateUser(Context.Message.Author.Id.ToString(), "AfkTime", now);
                await Context.Channel.SendMessageAsync(embed: builder.Build());
            }
        }
    }
}