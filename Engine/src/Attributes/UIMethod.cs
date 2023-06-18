using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UIMethod : Attribute
    {
        public int Order { get; init; }

        public UIMethod([CallerLineNumber]int order = 0)
        {
            Order = order;
        }
    }
}
