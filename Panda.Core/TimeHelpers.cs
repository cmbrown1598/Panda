using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace Panda
{
    static class TimeHelpers
    {
        public static ZonedDateTime NowInLocalTime()
        {
            return SystemClock.Instance.Now.InZone(DateTimeZoneProviders.Bcl.GetSystemDefault());
        }
    }
}
