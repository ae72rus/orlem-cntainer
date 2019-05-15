using System;
using System.Collections.Generic;
using OrlemSoftware.Basics.Core.Logging;

namespace OrlemSoftware.Basics.Core.Implementation
{
    class GlobalEventsInformer : IGlobalEventsInformer
    {
        private readonly List<IShutdown> _shutdowns = new List<IShutdown>();
        public event EventHandler Startup;
        public event EventHandler Shutdown;
        private readonly ILoggingService _loggingService;

        public GlobalEventsInformer(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public void RaiseStartup()
        {
            Startup?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseShutdown()
        {
            Shutdown?.Invoke(this, EventArgs.Empty);
            _shutdowns.ForEach(x => x.Shutdown());
        }

        public void AddShutdown(IShutdown shutdown)
        {
            _shutdowns.Add(shutdown);
        }

        public GlobalEventsInformer Clone(ILoggingService loggingService)
        {
            var retv = new GlobalEventsInformer(loggingService);
            foreach (var shutdown in _shutdowns)
            {
                retv._shutdowns.Add(shutdown);
            }

            return retv;
        }
    }
}