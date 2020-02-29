using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Tippy.Services;

namespace Tippy.Modules
{

    [Name("Images")]
    public class ImagesModule : ModuleBase<SocketCommandContext>
    {
        private static readonly Random rand = new Random();
        private Color GetRandomColour()
        {
            return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
        }

        public PictureService PictureService { get; set; }

        [Command("cat")]
        [Summary("Send a random cat image !")]
        public async Task CatAsync()
        {
            var stream = await PictureService.GetCatPictureAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("waifu")]
        [Summary("Send a random waifu image !")]
        public async Task WaifuAsync()
        {
            var rand = new Random();
            var randNum = rand.Next(199999);
            var stream = await PictureService.GetWaifuPictureAsync(randNum);
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "waifu.png");
        }

        [Command("halloween")]
        [Summary("Draws an image over a halloween border")]
        public async Task HalloweenAsync([Remainder] IUser member = null)
        {
            if (member == null) member = Context.User;
            var stream = await PictureService.GetHalloweenPictureAsync(member.GetAvatarUrl(ImageFormat.Png, 2048));
            var embed = new EmbedBuilder()
                .WithColor(GetRandomColour())
                .WithAuthor($"{member.Username}'s Halloween Picture Profile", member.GetAvatarUrl(ImageFormat.Png, 2048))
                .WithImageUrl("attachment://halloween.png")
                .WithFooter($"• Replying to {member.Username}#{member.Discriminator}", Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 2048));
            await Context.Channel.SendFileAsync(stream, "halloween.png", embed: embed.Build());
        }

        [Command("achievement")]
        [Summary("Sends a achievement with the text of your choice")]
        public async Task AchievementAsync([Remainder] String text = null)
        {
            if (text == null)
            {
                await ReplyAsync("Please provide a text!");
                return;
            }
            var stream = await PictureService.GetAchievementPictureAsync(Context.User.GetAvatarUrl(ImageFormat.Png, 2048), text);
            var embed = new EmbedBuilder()
                .WithColor(GetRandomColour())
                .WithImageUrl("attachment://achievement.png")
                .WithFooter($"• Replying to {Context.User.Username}#{Context.User.Discriminator}", Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 2048));
            await Context.Channel.SendFileAsync(stream, "achievement.png", embed: embed.Build());
        }

        [Command("beautiful")]
        [Summary("Draws a user's avatar over Gravity Falls \"Oh, this ? This is beautiful.\" meme")]
        public async Task BeautifulAsync([Remainder] IUser member = null)
        {
            if (member == null) member = Context.User;
            var stream = await PictureService.GetBeautifulPictureAsync(member.GetAvatarUrl(ImageFormat.Png, 2048));
            var embed = new EmbedBuilder()
                .WithColor(GetRandomColour())
                .WithImageUrl("attachment://beautiful.png")
                .WithFooter($"• Replying to {member.Username}#{member.Discriminator}", Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 2048));
            await Context.Channel.SendFileAsync(stream, "beautiful.png", embed: embed.Build());
        }

        [Command("triggered")]
        [Summary("Draws an image with the \"TRIGGERED\" gif")]
        public async Task TriggredAsync([Remainder] IUser member = null)
        {
            if (member == null) member = Context.User;
            var stream = await PictureService.GetTriggeredPictureAsync(member.GetAvatarUrl(ImageFormat.Png, 2048));
            var embed = new EmbedBuilder()
                .WithColor(GetRandomColour())
                .WithImageUrl("attachment://triggered.gif")
                .WithFooter($"• Replying to {member.Username}#{member.Discriminator}", Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 2048));
            await Context.Channel.SendFileAsync(stream, "triggered.gif", embed: embed.Build());
        }
    }
}
