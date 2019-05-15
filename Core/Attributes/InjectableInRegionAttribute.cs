using System;

namespace OrlemSoftware.Basics.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectableInRegionAttribute : Attribute
    {
        public Type RegionType { get; }

        public InjectableInRegionAttribute(Type region)
        {
            RegionType = region ?? throw new ArgumentNullException(nameof(region));
        }
    }
}