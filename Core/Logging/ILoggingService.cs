namespace OrlemSoftware.Basics.Core.Logging
{
    public interface ILoggingService
    {
        ILogger GetLogger(string loggerName);
    }
}