using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class Parameter : Attribute
    {
        public Parameter([CallerLineNumber] int order = 0)
        {
            Order = order;
        }

        public int Order { get; }
    }
}
