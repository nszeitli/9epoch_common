using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace _9epoch_common
{
    public static class DateHelper
    {
        public static DateTime ParseExact(string s, out bool ok, string format = "yyyy-MM-dd hh:mm tt")
        {
            DateTime output = DateTime.MinValue;
            ok = DateTime.TryParseExact(s, format, CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out output);

            return output;
        }

    }
}
