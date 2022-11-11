using System;

namespace AdrianMiasik.Components.Core.Helpers
{
    /// <summary>
    /// Helper methods for managing and dealing with DateTime
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Extension method that removes the milliseconds from the DateTime in question.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime TrimMilliseconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
                dateTime.Second, 0, dateTime.Kind);
        }
    }
}