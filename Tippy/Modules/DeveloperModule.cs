using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Tippy;
using Tippy.Database;
using Discord;

namespace DiscordBot_DotNet.Modules
{
    [Name("Developer")]
    public class DeveloperModule : ModuleBase<SocketCommandContext>
    {
        private Manager _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);

        public class ScriptGlobals
        {
            public SocketCommandContext ctx { get; set; }
            public IDiscordClient client { get; set; }
        }

        /*
        [Command("addxp")]
        [Summary("Kepo")]
        [RequireOwner(ErrorMessage = "You're not my developer!")]
        public async Task AddXpAsync(string id = null, string xp = null)
        {

        }*/
    }
}
