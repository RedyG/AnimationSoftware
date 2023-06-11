using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UIMethod : Attribute
    {
        public int Index { get; init; }

        public UIMethod(int index)
        {
            Index = index;
        }
    }
}
