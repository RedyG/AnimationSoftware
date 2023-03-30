using Engine.Attributes;
using Engine.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities
{
    public static class Extensions
    {
        public static string ToShaderFormat(this Guid guid)
        {
            return guid.ToString().Replace("-", "");
        }

        public static float GetAspectRatio(this SizeF size)
        {
            return size.Width / size.Height;
        }

        public static float GetAspectRatio(this Size size)
        {
            return (float)size.Width / (float)size.Height;
        }

        public static float GetAspectRatio(this Vector2 vector)
        {
            return vector.X / vector.Y;
        }

        public static ReadOnlyCollection<NamedParameter> GetParameters(this object obj)
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

            return parameters.AsReadOnly();
        }
    }
}
