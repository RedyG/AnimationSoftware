using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
