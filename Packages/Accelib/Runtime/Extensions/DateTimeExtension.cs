using System;

namespace Accelib.Extensions
{
    public static class DateTimeExtension
    {
        public static string SecToTime(int totalSecond)
        {
            var hour = totalSecond / 3600;

            totalSecond -= (hour * 3600);
            var min = totalSecond / 60;

            totalSecond -= min * 60;
            var sec = totalSecond;

            return $"{hour:00}:{min:00}:{sec:00}";
        }
        
        public static long UtcTotalSec() => (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
    }
}