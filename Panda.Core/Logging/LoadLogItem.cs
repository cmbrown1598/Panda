using NodaTime;

namespace Panda.Logging
{
    public class LoadLogItem
    {
        public static LoadLogItem Log(LogLevel level, string text)
        {
            return new LoadLogItem
            {
                Text = text,
                Level = level
            };
        }

        public LogLevel Level { get; set; }

        private LoadLogItem()
        {
            LogDate = TimeHelpers.NowInLocalTime();
        }



        public string Text { get; private set; }

        public ZonedDateTime LogDate { 
            get;
            private set;
        }
    }
}
