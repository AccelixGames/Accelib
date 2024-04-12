using System;

namespace Accelib.Extensions
{
    public static class DateTimeExtension
    {
        public static long UtcTotalSec()
        {
            return (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
        }
    }
}