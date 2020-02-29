using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tippy.Database;

namespace Tippy.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;
        private readonly Manager _manager;

        public CommandHandlingService(IServiceProvider services, DiscordSocketClient client)
        {
            _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _client = client;
            _services = services;
            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            var ch = message.Channel as SocketTextChannel;
            var data = await _manager.GetGuildByField("GuildId", ch.Guild.Id.ToString());
            if (message.Content.ToLower() == $"<@!{_client.CurrentUser.Id}>" || message.Content.ToLower() == $"<@{_client.CurrentUser.Id}>")
            {
                await message.Channel.SendMessageAsync($"> 👋 | Hello, **{message.Author.ToString()}** ! My prefix for this server is `{data.FirstOrDefault().Prefix}`");
            }

            var argPos = 0;
            if (!(message.HasStringPrefix(Assets.PREFIX, ref argPos) ||
             message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
             message.HasStringPrefix(data.FirstOrDefault().Prefix, ref argPos)) ||
             message.Author.IsBot) return;
            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
                return;

            if (result.IsSuccess)
                return;

            var results = result.ErrorReason.ToString().Replace("UnmetPrecondition: ", "");
            await context.Channel.SendMessageAsync($"{results}");
        }
    }
}
