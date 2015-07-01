using System;
using System.Collections.Generic;
using Panda.Logging;

namespace Panda
{
    public interface ILoadLog : IEnumerable<LoadLogItem>
    {
        void Info(string text);
        void Info(string format, params object[] objects);
        void Error(string text);
        void Error(Exception exception);
        void Error(string format, params object[] objects);
    }
}