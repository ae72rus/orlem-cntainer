using System;

namespace OrlemSoftware.Basics.Core.Logging
{
    public interface ILogger
    {
        string Name { get; }
        void Log(string message);
        void Log(Exception e);
        void Log(Exception e, string message);
        void Debug(string message);
    }
}