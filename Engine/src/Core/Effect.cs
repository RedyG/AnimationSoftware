using Engine.Graphics;
using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class Effect
    {
        // TODO: add helpter method like Paramter? GetParamterByName(string name)

        // TODO: optimize if no need to ping pong
        // TDOO: make the API better
        public abstract RenderResult Render(Surface activeSurface, Surface secondSurface);
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
