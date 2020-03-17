using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tippy.Services
{
    public class LevelingUtil
    {
        Dictionary<long, bool> spamFilter = new Dictionary<long, bool>();
        public string Cooldown(TimeSpan time, TimeSpan limit, string format)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            var estimated = (time + limit) - now;
            if (estimated > new TimeSpan(0, 0, 0, 0)) return estimated.ToString(format);
            else return "false";
        }
        public long xpToNextLevel(int level)
        {
            return 10 * (((long)Math.Pow(level, 2)) + 10 * level + 20);
        }

        public long levelToXp(int levels)
        {
            long xp = 0;
            for (int level = 0; level <= levels; level++)
            {
                xp += xpToNextLevel(level);
            }

            return xp;
        }

        public int xpToLevels(long totalXp)
        {
            bool calculating = true;
            int level = 0;

            while (calculating)
            {
                long xp = levelToXp(level);

                if (totalXp < xp)
                {
                    calculating = false;
                }
                else
                {
                    level++;
                }
            }

            return level;
        }

        public long remainingXp(long totalXp)
        {
            int level = xpToLevels(totalXp);
            if (level == 0) return totalXp;
            long xp = levelToXp(level);

            return totalXp - xp + xpToNextLevel(level);
        }

        public int randomXp(int min, int max)
        {
            Random random = new Random();
            return random.Next((max - min) + 1) + min;
        }

        public void addUserToSpamFilter(long userId)
        {
            if (!isUserHasSpamFilter(userId))
            {
                spamFilter.Add(userId, true);
            }

            Task.Delay(60000).ContinueWith((Task) => {
                if (isUserHasSpamFilter(userId))
                {
                    spamFilter.Remove(userId);
                }
                return Task.CompletedTask;
            });
        }

        public bool isUserHasSpamFilter(long userId)
        {
            return spamFilter.ContainsKey(userId);
        }
    }
}
