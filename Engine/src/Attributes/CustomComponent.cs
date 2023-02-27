using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomComponent : Attribute
    {
        Type Component;

        public CustomComponent(Type component)
        {
            Component = component;
        }
    }
}
