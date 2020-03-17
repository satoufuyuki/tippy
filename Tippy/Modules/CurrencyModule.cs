using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tippy.Database;
using Tippy.Services;

namespace Tippy.Modules
{
	[Name("Currency")]
	public class CurrencyModule : ModuleBase<SocketCommandContext>
	{
		private Manager _manager = new Manager(Assets.MONGO_DB_CONNECTION_QUERY);
		private LevelingUtil util = new LevelingUtil();

		[Command("daily")]
		[Alias("dailies")]
		[Summary("Collect your daily reward!")]
		public async Task DailyAsync()
		{
			var data = await _manager.GetUsersByField("UserId", Context.Message.Author.Id.ToString());
			TimeSpan now = DateTime.Now.TimeOfDay;
			var cooldown = util.Cooldown(data.FirstOrDefault().LastDaily, new TimeSpan(24, 0, 0), @"hh\h\ mm\m\ ss\s");
			if (cooldown != "false")
			{
				await Context.Channel.SendMessageAsync($"<@{Context.Message.Author.Id}>, you can collect your daily again in **{cooldown}**");
				return;
			}

			await _manager.UpdateUser(Context.User.Id.ToString(), "LastDaily", now.ToString());
			await _manager.UpdateUser(Context.User.Id.ToString(), "NextDaily", (now + new TimeSpan(24, 0, 0)).ToString());
			await Context.Message.Channel.SendMessageAsync($"💵 | <@{Context.Message.Author.Id}> collected $200 from daily reward !");
			await _manager.UpdateUser(Context.User.Id.ToString(), "Money", (data.FirstOrDefault().Money + 250).ToString());
		}

		/* /// Discontinued ///
		[Command("rep")]
		[Summary("Give a reputation to other user!")]
		public async Task RepAsync([Remainder] IUser member = null)
		{
			if (member == null)
			{
				await Context.Channel.SendMessageAsync("> <:no:615730302936940554> | Please mention the user !");
				return;
			}
			else if (member.Id == Context.Message.Author.Id)
			{
				await Context.Channel.SendMessageAsync("> <:no:615730302936940554> | You can't give reputation to yourself !");
				return;
			}

			var authorData = await _manager.GetUsersByField("UserId", Context.Message.Author.Id.ToString());
			var memberData = await _manager.GetUsersByField("UserId", member.Id.ToString());
			if (authorData.FirstOrDefault() == null)
			{
				User newData = new User
				{
					UserId = Context.Message.Author.Id,
					Rep = 0,
					Xp = 0,
					Level = 0,
					Money = 0,
					LastDaily = new TimeSpan(0, 0, 0),
					LastRep = new TimeSpan(0, 0, 0),
					LastWork = new TimeSpan(0, 0, 0)
				};

				await _manager.InsertUser(newData);
			}

			if (memberData.FirstOrDefault() == null)
			{
				User newData = new User
				{
					UserId = member.Id,
					Rep = 0,
					Xp = 0,
					Level = 0,
					Money = 0,
					LastDaily = new TimeSpan(0, 0, 0),
					LastRep = new TimeSpan(0, 0, 0),
					LastWork = new TimeSpan(0, 0, 0)
				};

				await _manager.InsertUser(newData);
			}

			if (authorData.FirstOrDefault().LastRep != new TimeSpan(0, 0, 0))
			{
				await Context.Channel.SendMessageAsync($"<@{Context.Message.Author.Id}>, you can give reputation again in **{authorData.FirstOrDefault().LastDaily.ToString(@"hh\h\ mm\m\ ss\s")}**");
				return;
			}

			await SetTime(Context.Message.Author.Id.ToString(), "LastRep", 24);
			await _manager.UpdateUser(member.Id.ToString(), "Rep", (memberData.FirstOrDefault().Rep + 1).ToString());
			await Context.Message.Channel.SendMessageAsync($"Hey, <@{member.Id}> ! **{Context.Message.Author.Username}** gaved you reputation !");
			return;
		}
		*/
	}
}
