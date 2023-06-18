using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing.Drawing2D;
using Engine.Attributes;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Engine.Effects
{
    // TODO: time displacement with alpha map thingy

    public class NoChange : VideoEffect
    {
        static ShaderProgram shader = Surface.CompileShader(@"
vec4 surface()
{
vec2 offset = 1.0 / iResolution;
return texture(input, uv + offset);
}
        ");
        public override RenderResult Render(RenderArgs args)
        {
            args.SurfaceB.Bind(FramebufferTarget.Framebuffer);
            GraphicsApi.DrawSurface(MatrixBuilder.Empty, args.SurfaceA, shader);


            return new RenderResult(true);
        }
    }
}
