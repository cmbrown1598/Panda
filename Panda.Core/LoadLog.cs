using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Panda
{
    public class LoadLog : List<LoadLogItem>, ILoadLog
    {
        public void Info(string text)
        {
            Add(LoadLogItem.Log(LogLevel.Info, text));
        }
        public void Info(string format, params object[] objects)
        {
            Info(string.Format(format, objects));
        }
        public void Error(string text)
        {
            Add(LoadLogItem.Log(LogLevel.Error, text));
        }

        public void Error(Exception exception)
        {
            Add(LoadLogItem.Log(LogLevel.Error, exception.ToString()));
        }

        public void Error(string format, params object[] objects)
        {
            Error(string.Format(format, objects));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            ForEach(a =>
            {
                sb.AppendFormat("{0}|{1}|{2}{3}", a.LogDate.ToString("F", CultureInfo.InvariantCulture), a.Level.ToString().ToUpper(), a.Text, Environment.NewLine);
            });
            return sb.ToString();
        }
    }
}