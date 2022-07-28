#nullable enable
using System;

namespace Cyberultimate.Unity
{
    [AttributeUsage(AttributeTargets.Field| AttributeTargets.Property)]
    public class CommandPropertyAttribute: Attribute
    {
        public readonly bool Set;
        public readonly bool Get;
        public readonly string? Name;

        public CommandPropertyAttribute(bool get=false, bool set=false,
            string? name=null)
        {
            Get = get;
            Set = set;
            Name = name;
        }


      
    }
}