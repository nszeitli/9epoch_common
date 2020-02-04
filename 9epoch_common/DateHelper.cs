using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace _9epoch_common
{
    public static class DateHelper
    {
        public static bool ParseExact(string s, out DateTime dateTime, string format = "yyyy-MM-dd hh:mm tt")
        {
            dateTime = DateTime.MinValue;
            bool ok = DateTime.TryParseExact(s, format, CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out dateTime);

            return ok;
        }

    }
}
