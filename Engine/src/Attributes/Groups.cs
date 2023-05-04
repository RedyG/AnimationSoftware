using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Attributes
{
    public class BeginGroup : Attribute
    {
        public string Name { get; init; }

        public BeginGroup(string name)
        {
            Name = name;
        }
    }
    public class EndGroup : Attribute { }
}
