using OrlemSoftware.Basics.Core.Logging;

namespace ImplementationTests
{
    class LoggingService : ILoggingService
    {
        public ILogger GetLogger(string loggerName)
        {
            return new Logger();
        }
    }
}