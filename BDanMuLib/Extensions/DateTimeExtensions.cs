using System;

namespace LiveCore.Extensions
{
    internal static class DateTimeExtensions
    {
        internal static DateTime ConvertStringToDateTime(this string timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1).ToLocalTime();
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }
}
