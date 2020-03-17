using Discord;
using Discord.Commands;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tippy.Database;

namespace Tippy.Modules
{
	[Name("Developer")]
    public class DeveloperModule : ModuleBase<SocketCommandContext>
    {
        private Manager _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);

		public class ScriptGlobals
        {
            public SocketCommandContext ctx { get; internal set; }
            public IDiscordClient client { get; internal set; }
			public Manager manager { get; internal set; }
        }

		[Command("eval")]
		[Alias("ev", "e")]
        [Summary("Evaluate")]
        [RequireOwner(ErrorMessage = "You're not my developer!")]
        public async Task EvalAsync([Remainder] string code = null)
        {
			if (code == null)
			{
				return;
			}

			var options = ScriptOptions.Default.AddImports("System.Collections.Generic");
			var globals = new ScriptGlobals { ctx = Context, client = Context.Client, manager = _manager };
			
			try
			{
				var start = DateTime.Now.Millisecond;
				var script = CSharpScript.Create(code, options, typeof(ScriptGlobals));
				var scriptState = await script.RunAsync(globals);
				var returnValue = scriptState.ReturnValue;
				if (returnValue != null)
					await Context.Channel.SendMessageAsync($"<:yes:615730280619048970> **Success :**\n" +
						"```\n" +
						returnValue.ToString() +
						"\n```");
			}
			catch (Exception ex)
			{
				await Context.Channel.SendMessageAsync("<:no:615730302936940554> **Error :**\n" +
					"```ini\n" +
					ex.Message +
					"\n```");
			}
		}
	}
}
