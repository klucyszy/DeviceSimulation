using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace mekg.DeviceSimulation.Helpers
{
    public static class Converter
    {
        public static DateTime UnixEpochToDateTime(long seconds)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return origin.AddSeconds(seconds);
        }

        public static List<double> SampleDataConverter(string txt)
        {
            var array = new List<string>(txt.Split(','));
            var dataFormat = new NumberFormatInfo
            {
                NegativeSign = "-",
                NumberDecimalSeparator = "."
            };

            return array.Select((i) => double.Parse(i, dataFormat)).ToList();
        }
    }
}
