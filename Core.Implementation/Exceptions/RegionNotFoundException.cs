using System;

namespace OrlemSoftware.Basics.Core.Implementation.Exceptions
{
    public class RegionNotFoundException : Exception
    {
        public RegionNotFoundException()
        {

        }

        public RegionNotFoundException(string message)
        : base(message)
        {

        }
    }
}