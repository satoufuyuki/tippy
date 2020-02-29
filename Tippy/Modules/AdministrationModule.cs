using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tippy.Database;

namespace Tippy.Modules
{
    [Name("Administration")]
    public class AdministrationModule : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }
        private readonly CommandService _service;
        private static readonly Random rand = new Random();
        private Manager _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
        private Color GetRandomColour()
        {
            return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
        }
        public AdministrationModule(CommandService service)
        {
            _service = service;
        }

        [Command("welcome")]
        [Summary("Greet a member when the member joined your server!")]
        [RequireUserPermission(Discord.GuildPermission.ManageGuild, ErrorMessage = "You're not a administrator in this server. (`MANAGE_SERVER` Permission is missing)")]
        public async Task WelcomeAsync(string type = null, [Remainder] ITextChannel channel = null)
        {
            {
                var data = await _manager.GetGuildByField("GuildId", Context.Guild.Id.ToString());
                if (type == null)
                {
                    await ReplyAsync($"Please choose an option : `{data.FirstOrDefault().Prefix}welcome <set|test|disable|enable>`");
                    return;
                }
                else if (type.Contains("test"))
                {
                    var stream = await PictureService.GetWelcomeCanvasAsync(Context.User.GetAvatarUrl(Discord.ImageFormat.Png, 2048), $"{Context.User.Username}#{Context.User.Discriminator}", $"This server now has {Context.Guild.MemberCount} members");
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    if (data.FirstOrDefault().EnabledWelcome == false)
                    {
                        await ReplyAsync("WARN: The welcome feature is disabled, if a member join your server, I will not greet that member.");
                    }
                    if (data.FirstOrDefault().WelcomeChannel == "")
                    {
                        await Context.Channel.SendFileAsync(stream, "welcome.png");
                        return;
                    }
                    var ch = Context.Guild.GetTextChannel((ulong)Convert.ToInt64(data.FirstOrDefault().WelcomeChannel));
                    if (ch == null)
                    {
                        await Context.Channel.SendFileAsync(stream, "welcome.png");
                        return;
                    }
                    await ch.SendFileAsync(stream, "welcome.png", $"Welcome to **{Context.Guild.Name}**, **{Context.User.Username}#{Context.User.Discriminator}**");
                    if (ch.Id != Context.Channel.Id) await Context.Channel.SendMessageAsync($"See <#{ch.Id}> to view a welcome test");
                    return;
                }
                else if (type.Contains("set"))
                {
                    if (channel == null)
                    {
                        await ReplyAsync("Please mention a channel!");
                        return;
                    }
                    var ch = Context.Message.MentionedChannels.ElementAt(0);
                    if (!Context.Guild.CurrentUser.GetPermissions(ch).Has(ChannelPermission.SendMessages) || !Context.Guild.CurrentUser.GetPermissions(ch).Has(ChannelPermission.AttachFiles))
                    {
                        await ReplyAsync($"I'm not have permissions `SEND_MESSAGES` or `ATTACH_FILES` in <#{channel.Id}>");
                        return;
                    }

                    await _manager.UpdateGuild(Context.Guild.Id.ToString(), "WelcomeChannel", ch.Id.ToString());
                    await ReplyAsync($"Alright, now when a member join this server, I'll greet them in <#{channel.Id}>");
                }
                else if (type.Contains("disable"))
                {
                    if (data.FirstOrDefault().EnabledWelcome == false)
                    {
                        await ReplyAsync("Welcome feature is already disabled!");
                        return;
                    }
                    else
                    {
                        await _manager.UpdateGuild(Context.Guild.Id.ToString(), "EnabledWelcome", "false");
                        await ReplyAsync($"Welcome feature has been disabled. You can enable this feature using `{Assets.PREFIX}welcome enable`");
                        return;
                    }
                }
                else if (type.Contains("enable"))
                {
                    if (data.FirstOrDefault().EnabledWelcome == true)
                    {
                        await ReplyAsync("Welcome feature is already enabled!");
                        return;
                    }
                    else
                    {
                        await _manager.UpdateGuild(Context.Guild.Id.ToString(), "EnabledWelcome", "true");
                        await ReplyAsync($"Welcome module has been enabled. You can disable this feature using `{Assets.PREFIX}welcome disable`");
                        return;
                    }
                }
                else
                {
                    await ReplyAsync($"Please choose an option : $`{data.FirstOrDefault().Prefix}welcome <set|test|disable|enable>`");
                    return;
                }
            }
        }

        [Command("leave")]
        [Alias("goodbye", "bye", "farewell")]
        [Summary("Configure the server goodbye message")]
        [RequireUserPermission(Discord.GuildPermission.ManageGuild, ErrorMessage = "You're not a administrator in this server. (`MANAGE_SERVER` Permission is missing)")]
        public async Task GoodbyeAsync(string type = null, [Remainder] ITextChannel channel = null)
        {
            var data = await _manager.GetGuildByField("GuildId", Context.Guild.Id.ToString());
            {
                if (type == null)
                {
                    await ReplyAsync($"Please choose an option : `{data.FirstOrDefault().Prefix}leave <set|test|disable|enable>`");
                    return;
                }
                else if (type.Contains("test"))
                {
                    var stream = await PictureService.GetLeaveCanvasAsync(Context.User.GetAvatarUrl(Discord.ImageFormat.Png, 2048), $"{Context.User.Username}#{Context.User.Discriminator}", $"This server now has {Context.Guild.MemberCount} members");
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    if (data.FirstOrDefault().EnabledLeave == false)
                    {
                        await ReplyAsync("WARN: The leave feature is disabled, if a member leave your server, I will not say goodbye to that member.");
                    }
                    if (data.FirstOrDefault().LeaveChannel == "")
                    {
                        await Context.Channel.SendFileAsync(stream, "leave.png");
                        return;
                    }
                    var ch = Context.Guild.GetTextChannel((ulong)Convert.ToInt64(data.FirstOrDefault().LeaveChannel));
                    if (ch == null)
                    {
                        await Context.Channel.SendFileAsync(stream, "leave.png");
                        return;
                    }
                    await ch.SendFileAsync(stream, "leave.png", $"Goodbye, **{Context.User.Username}#{Context.User.Discriminator}**");
                    if (ch.Id != Context.Channel.Id) await Context.Channel.SendMessageAsync($"See <#{ch.Id}> to view a leave test");
                    return;
                }
                else if (type.Contains("set"))
                {
                    if (channel == null)
                    {
                        await ReplyAsync("Please mention a channel!");
                        return;
                    }

                    if (!Context.Guild.CurrentUser.GetPermissions(channel).Has(ChannelPermission.SendMessages) || !Context.Guild.CurrentUser.GetPermissions(channel).Has(ChannelPermission.AttachFiles))
                    {
                        await ReplyAsync($"I'm not have permissions `SEND_MESSAGES` or `ATTACH_FILES` in <#{channel.Id}>");
                        return;
                    }

                    await _manager.UpdateGuild(Context.Guild.Id.ToString(), "LeaveChannel", channel.Id.ToString());
                    await ReplyAsync($"Alright, now when a member is leave from your server, I'll say goodbye to them in  <#{channel.Id}>");

                }
                else if (type.Contains("disable"))
                {
                    if (data.FirstOrDefault().EnabledLeave == false)
                    {
                        await ReplyAsync("Farewell feature is already disabled!");
                        return;
                    }
                    else
                    {
                        await _manager.UpdateGuild(Context.Guild.Id.ToString(), "EnabledLeave", "false");
                        await ReplyAsync($"Farewell feature has been disabled. You can enable this feature using `{Assets.PREFIX}leave enable`");
                        return;
                    }
                }
                else if (type.Contains("enable"))
                {
                    if (data.FirstOrDefault().EnabledLeave == true)
                    {
                        await ReplyAsync("Farewell feature is already enabled!");
                        return;
                    }
                    else
                    {
                        await _manager.UpdateGuild(Context.Guild.Id.ToString(), "EnabledLeave", "true");
                        await ReplyAsync($"Farewell feature has been enabled. You can disable this feature using `{Assets.PREFIX}welcome disable`");
                        return;
                    }
                }
                else
                {
                    await ReplyAsync($"Please choose an option : $`{data.FirstOrDefault().Prefix}leave <set|test|disable|enable>`");
                    return;
                }
            }
        }

        [Command("kick")]
        [Summary("Kick a member in your server!")]
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You not have permission to do this. `(You need kick members permission to continue this command)`")]
        [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage = "I'm not have permission to do this. `(Kick Member permission is missing for me)`")]
        public async Task KickAsync(IGuildUser member, [Remainder] string reason = null)
        {
            if (reason == null) reason = "No reason provided.";
            try
            {
                await member.KickAsync(reason);
            }
            catch (Exception e)
            {
                if (e.Message == "The server responded with error 403: Forbidden")
                {
                    await Context.Channel.SendMessageAsync($"**{member.Username}#{member.Discriminator}** cannot be kicked. Maybe their role is higher than me?");
                    return;
                };
            }
            await Context.Channel.SendMessageAsync($"<:yes:596220835254108192> | **{member.Username}#{member.Discriminator}** have been **__KICKED__** from **{Context.Guild.Name}**");
            var embed = new EmbedBuilder()
            {
                Color = GetRandomColour()
            };
            embed.WithAuthor("👢 KICKED", Context.Message.Author.GetAvatarUrl(ImageFormat.Png, 2048))
                .WithCurrentTimestamp()
                .WithDescription($"You have been kicked from **{Context.Guild.Name}** \nReason : \n```{reason}```")
                .WithFooter($"Regards, {Context.Guild.Name} Staff", Context.Guild.IconUrl);
            try
            {
                await member.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                if (e.Message == "The server responded with error 50007: Cannot send messages to this user")
                {
                    return;
                }
            };
        }

        [Command("ban")]
        [Summary("Ban a member in your server!")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You not have permission to do this. `(You need ban members permission to continue this command)`")]
        [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "I'm not have permission to do this. `(Ban Member permission is missing for me)`")]
        public async Task BanAsync(IGuildUser member, [Remainder] string reason = null)
        {
            if (reason == null) reason = "No reason provided.";
            try
            {
                await member.BanAsync(7, reason);
            }
            catch (Exception e)
            {
                if (e.Message == "The server responded with error 403: Forbidden")
                {
                    await Context.Channel.SendMessageAsync($"**{member.Username}#{member.Discriminator}** cannot be banned. Maybe their role is higher than me?");
                    return;
                };
            }
            await Context.Channel.SendMessageAsync($"<:yes:596220835254108192> | **{member.Username}#{member.Discriminator}** have been **__BANNED__** from **{Context.Guild.Name}**");
            var embed = new EmbedBuilder()
            {
                Color = GetRandomColour()
            };
            embed.WithAuthor("👢 KICKED", Context.Message.Author.GetAvatarUrl(ImageFormat.Png, 2048))
                .WithCurrentTimestamp()
                .WithDescription($"You have been banned from **{Context.Guild.Name}** \nReason : \n```{reason}```")
                .WithFooter($"Regards, {Context.Guild.Name} Staff", Context.Guild.IconUrl);
            try
            {
                await member.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                if (e.Message == "The server responded with error 50007: Cannot send messages to this user")
                {
                    return;
                }
            };
        }

        [Command("prefix")]
        [Summary("Set a custom prefix for this bot in your server!")]
        public async Task PrefixAsync(string newPrefix = null)
        {
            var data = _manager.GetGuildByField("GuildId", Context.Guild.Id.ToString());
            if (Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageGuild == false && Context.User.Id.ToString() != Assets.OWNER)
            {
                await Context.Channel.SendMessageAsync("You not have permission to do this. `(You need manage server permission to continue this command)`");
                return;
            }

            if (string.IsNullOrEmpty(newPrefix))
            {
                await Context.Channel.SendMessageAsync("Please provide a new prefix!");
                return;
            }

            newPrefix = newPrefix.Trim();
            newPrefix = newPrefix.Split(" ")[0];
            if (newPrefix == Assets.PREFIX)
            {
                await _manager.UpdateGuild(Context.Guild.Id.ToString(), "Prefix", Assets.PREFIX);
                await Context.Channel.SendMessageAsync($"Prefix have been resetted to `{Assets.PREFIX}`");
                return;
            } else
            {
                await _manager.UpdateGuild(Context.Guild.Id.ToString(), "Prefix", newPrefix);
                await Context.Channel.SendMessageAsync($"Prefix have been changed to `{newPrefix}`");
                return;
            }
        }
    }
}
