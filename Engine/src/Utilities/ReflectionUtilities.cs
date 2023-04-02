using Engine.Attributes;
using Engine.Core;
using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities
{
    public static class ReflectionUtilities
    {
        public static ParameterList GetParameters(object obj)
        {
            // TODO: maybe optimize this code a little so we don't query the attribute "Param" twice.
            // TODO: cache this per-Type so that we don't this work multiple times for the same types.
            // TODO: add parameter groups or a way to nest them.
            // TODO: handle duplicate names.

            List<NamedParameter> parameters = new();

            var properties = obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => Attribute.IsDefined(property, typeof(Param)) && typeof(Parameter).IsAssignableFrom(property.PropertyType))
                .OrderBy(property => ((Param)property.GetCustomAttributes(typeof(Param), false).Single()).Order);

            foreach (var property in properties)
            {
                var name = ((Param)property.GetCustomAttributes(typeof(Param), false).Single()).CustomName ?? StringUtilities.UnPascalCase(property.Name);
                parameters.Add(new NamedParameter(name, (Parameter)property.GetValue(obj)!));
            }

            return new ParameterList(parameters);
        }
    }
}
