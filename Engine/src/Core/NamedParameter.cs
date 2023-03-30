using Engine.Attributes;
using Engine.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public readonly struct NamedParameter
    {   
        public string Name { get; }
        public Parameter Parameter { get; }

        public NamedParameter(string name, Parameter parameter)
        {
            Name = name;
            Parameter = parameter;
        }
    }
}
