using Engine.Attributes;
using Engine.Graphics;
using Engine.OpenGL;
using Engine.Utilities;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class Effect
    {
        // TODO: add helpter method like Paramter? GetParamterByName(string name)
        private Type _type;

        public string Name { get; }

        public Dictionary<string, Parameter> Parameters { get; }


        // TODO: optimize if no need to ping pong
        // TDOO: make the API better
        public abstract RenderResult Render(Surface activeSurface, Surface secondSurface, SizeF size);

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

        private Dictionary<string, Parameter> GetParameters()
        {
            Dictionary<string, Parameter> parameters = new();
            foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (typeof(Parameter).IsAssignableFrom(property.PropertyType))
                {
                    parameters.Add(StringUtilities.UnPascalCase(property.Name), (Parameter)property.GetValue(this)!);
                }
            }
            foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (typeof(Parameter).IsAssignableFrom(field.FieldType))
                {
                    parameters.Add(StringUtilities.UnPascalCase(field.Name), (Parameter)field.GetValue(this)!);
                }
            }
            return parameters;
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
