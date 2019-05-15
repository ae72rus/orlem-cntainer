using System;
using OrlemSoftware.Basics.Core.Logging;

namespace OrlemSoftware.Basics.Core.Implementation
{
    public class MockLoggingService : ILoggingService
    {
        private class MockLogger : ILogger
        {
            public void Log(string message)
            {

            }

            public void Log(Exception e)
            {

            }

            public void Log(Exception e, string message)
            {

            }

            public void Debug(string message)
            {

            }

            public string Name => nameof(MockLogger);
        }

        private static readonly MockLogger _logger = new MockLogger();
        public ILogger GetLogger(string loggerName)
        {
            return _logger;
        }
    }
}