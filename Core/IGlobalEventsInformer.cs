using System;

namespace OrlemSoftware.Basics.Core
{
    public interface IGlobalEventsInformer
    {
        event EventHandler Startup;
        event EventHandler Shutdown;
    }
}