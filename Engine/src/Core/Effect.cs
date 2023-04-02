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
        public string Name { get; }

        public ParameterList Parameters { get; }

        // TODO: optimize if no need to ping pong
        // TDOO: make the API better
        public abstract RenderResult Render(RenderArgs args);
        protected abstract ParameterList InitParameters();
        protected virtual string GetName() => StringUtilities.UnPascalCase(GetType().Name);

        public Effect()
        {
            Name = GetName();

            Parameters = InitParameters();
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
