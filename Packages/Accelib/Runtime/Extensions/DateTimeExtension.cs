using System;

namespace Accelib.Extensions
{
    public static class DateTimeExtension
    {
        public enum TimeDisplayMode {Hour, Min, Sec}
        
        public static string SecToTime(int totalSecond, TimeDisplayMode mode = TimeDisplayMode.Hour)
        {
            var hour = totalSecond / 3600;

            totalSecond -= (hour * 3600);
            var min = totalSecond / 60;

            totalSecond -= min * 60;
            var sec = totalSecond;

            if(mode == TimeDisplayMode.Hour)
                return $"{hour:00}:{min:00}:{sec:00}";
            if(mode == TimeDisplayMode.Min)
                return $"{min:00}:{sec:00}";
            return $"{sec:00}";
        }
        
        public static long UtcTotalSec() => (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
        
        public static long TotalSec(this DateTime dateTime) => (long)(dateTime - DateTime.UnixEpoch).TotalSeconds;
    }
}