using System;

namespace Taskbarv3.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string TranslateDayToSwedish(this DateTime dateTime)
        {
            var englishDay = dateTime.DayOfWeek.ToString();
            switch (englishDay)
            {
                case "Monday":
                    return "måndag";
                case "Tuesday":
                    return "tisdag";
                case "Wednesday":
                    return "onsdag";
                case "Thursday":
                    return "torsdag";
                case "Friday":
                    return "fredag";
                case "Saturday":
                    return "lördag";
                case "Sunday":
                    return "söndag";
                default:
                    return englishDay;
            }
        }
    }
}
