using ImageMagick;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tippy
{
    public class PictureService
    {
        private readonly HttpClient _http;
        private readonly string EMILLIA_BASE_URI = "https://emilia.shrf.xyz/api/";
        public PictureService(HttpClient http)
            => _http = http;

        public PictureService()
        {
        }

        public byte[] GetWelcomeImage()
        {
            using (MagickImage image = new MagickImage(new MagickColor("#ff00ff"), 1024, 450))
            {
                {
                    new Drawables()
                    .FontPointSize(20)
                    .Font("Comic Sans")
                    .StrokeColor(new MagickColor("black"))
                    .FillColor(MagickColors.White)
                    .TextAlignment(TextAlignment.Center)
                    .Text(512, 355, "WELCOME")
                    .Draw(image);
                    return image.ToByteArray();
                }
            }
        }

        public async Task<Stream> GetCatPictureAsync()
        {
            var resp = await _http.GetAsync("https://cataas.com/cat");
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetWaifuPictureAsync(int num)
        {
            var resp = await _http.GetAsync($"https://www.thiswaifudoesnotexist.net/example-{num}.jpg");
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetHalloweenPictureAsync(String image)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Assets.EMILIA_KEY}");
            var resp = await client.GetAsync(EMILLIA_BASE_URI + "halloween?image=" + image);
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetAchievementPictureAsync(String image, string text)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Assets.EMILIA_KEY}");
            var resp = await client.GetAsync(EMILLIA_BASE_URI + "achievement?image=" + image + $"&text={text}");
            Console.WriteLine(resp.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetBeautifulPictureAsync(String image)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Assets.EMILIA_KEY}");
            var resp = await client.GetAsync(EMILLIA_BASE_URI + "beautiful?image=" + image);
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetTriggeredPictureAsync(String image)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Assets.EMILIA_KEY}");
            var resp = await client.GetAsync(EMILLIA_BASE_URI + "triggered?image=" + image);
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetWelcomeCanvasAsync(String image, String name, String message)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Assets.EMILIA_KEY}");
            var resp = await client.GetAsync(EMILLIA_BASE_URI + "greetings?type=welcome&image=" + image + "&message=" + message + "&name=" + name + "&messageColor=#000000" + "&backgroundImage=" + Assets.WELCOME_IMAGE);
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetLeaveCanvasAsync(String image, String name, String message)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Assets.EMILIA_KEY}");
            var resp = await client.GetAsync(EMILLIA_BASE_URI + "greetings?type=farewell&image=" + image + "&message=" + message + "&name=" + name + "&messageColor=#000000" + "&backgroundImage=" + Assets.WELCOME_IMAGE);
            return await resp.Content.ReadAsStreamAsync();
        }
    }
}
