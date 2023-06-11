using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Description : Attribute
    {
        public string? Name { get; init; }
        public string? Category { get; init; }
        public bool Hidden { get; init; } = false;
    }
}
