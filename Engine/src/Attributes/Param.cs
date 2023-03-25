using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Attributes
{
    // TODO: support fields too
    [AttributeUsage(AttributeTargets.Property)]
    public class Param : Attribute
    {
        public Param([CallerLineNumber] int order = 0)
        {
            Order = order;
        }

        public Param(string customName, [CallerLineNumber] int order = 0)
        {
            CustomName = customName;
            Order = order;
        }


        public string? CustomName { get; }
        public int Order { get; }
    }
}
