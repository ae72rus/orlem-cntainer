using System;
using OrlemSoftware.Basics.Core.Logging;

namespace ImplementationTests
{
    class Logger : ILogger
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

        public string Name { get; }
    }
}