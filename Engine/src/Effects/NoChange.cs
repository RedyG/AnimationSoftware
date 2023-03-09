using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    // TODO: time displacement with alpha map thingy
    public class NoChange : Effect
    {
        ShaderProgram shader;


        public override RenderResult Render(Surface mainSurface, Surface secondSurface)
        {
            return new RenderResult(true);
        }

        public NoChange()
        {

        }
    }
}
