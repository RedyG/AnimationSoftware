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
        public abstract RenderResult Render(RenderArgs args);

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

            Parameters = this.GetParameters();
        }

        private string GetEffectName()
        {
            var customName = _type.GetCustomAttribute(typeof(CustomName)) as CustomName;

            if (customName != null)
                return customName.Name;

            return StringUtilities.UnPascalCase(_type.Name);
        }
    }

    public readonly struct RenderArgs
    {
        public Timecode Time { get; }
        public Surface SurfaceA { get; }
        public Surface SurfaceB { get; }
        public Layer Layer { get; }

        public RenderArgs(Timecode time, Layer layer, Surface surfaceA, Surface surfaceB)
        {
            Time = time;
            SurfaceA = surfaceA;
            SurfaceB = surfaceB;
            Layer = layer;
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
