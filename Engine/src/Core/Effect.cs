using Engine.Attributes;
using Engine.Graphics;
using Engine.OpenGL;
using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine.Core
{
    public abstract class Effect
    {
        private Type _type;

        public string Name { get; }

        public ReadOnlyCollection<NamedParameter> Parameters { get; }


        // TODO: optimize if no need to ping pong
        // TDOO: make the API better
        public abstract RenderResult Render(Surface activeSurface, Surface secondSurface, SizeF size);

        public Parameter? GetParameter(string name)
        {
            foreach (var namedParameter in Parameters)
            {
                if (namedParameter.Name == name)
                    return namedParameter.Parameter;
            }
            return null;
        }


        public Effect()
        {
            _type = GetType();

            Name = GetEffectName();

            Parameters = GetParameters();
        }

        private string GetEffectName()
        {
            var customName = _type.GetCustomAttribute(typeof(CustomName)) as CustomName;

            if (customName != null)
                return customName.Name;

            return StringUtilities.UnPascalCase(_type.Name);
        }

        private ReadOnlyCollection<NamedParameter> GetParameters()
        {
            // TODO: maybe optimize this code a little so we don't query the attribute "Param" twice.
            List<NamedParameter> parameters = new();

            var properties = _type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => Attribute.IsDefined(property, typeof(Param)) && typeof(Parameter).IsAssignableFrom(property.PropertyType))
                .OrderBy(property => ((Param)property.GetCustomAttributes(typeof(Param), false).Single()).Order);

            foreach (var property in properties)
            {
                var name = ((Param)property.GetCustomAttributes(typeof(Param), false).Single()).CustomName ?? StringUtilities.UnPascalCase(property.Name);
                parameters.Add(new NamedParameter(name, (Parameter)property.GetValue(this)!));
            }

            return parameters.AsReadOnly();
        }
    }

    public readonly struct RenderResult
    {
        public bool SwapSurfaces { get; }

        public RenderResult(bool swapSurfaces)
        {
            SwapSurfaces = swapSurfaces;
        }
    }
}
