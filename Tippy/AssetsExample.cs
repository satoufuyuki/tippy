using Discord;

namespace Tippy
{
    public class AssetsExample // Rename the file to Assets.cs and rename the class to Assets
    {
        public static readonly string TOKEN = "Put your discord bot token here";
        public static readonly string EMILIA_KEY = "Put your emilia api key here"; // API FROM : https://emilia.shrf.xyz/
        public static readonly string PREFIX = "Put your bot prefix here";
        public static readonly string WELCOME_IMAGE = "https://cdn.glitch.com/5785851c-6e3e-47a4-b379-7ea0b2b52d29%2Ff491d457-57e4-41d7-927b-d8eb9bfa5b55.image.png?v=1572134071161"; // You can change this to another image
        public static readonly string OWNER = "Your discord user id";
        public static readonly string MONGO_DB_CONNECTION_QUERY = "mongodb://localhost:27017/"; // Put your mongodb connection string here!
    }
}
