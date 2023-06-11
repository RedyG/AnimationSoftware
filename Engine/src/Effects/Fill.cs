using Engine.Core;
using Engine.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Engine.OpenGL;
using Engine.UI;

namespace Engine.Effects
{
    public class Fill : VideoEffect
    {
        public Parameter<Color4> Color { get; } = new();

        private static ShaderProgram _shader = Surface.CompileShader(@"
            uniform vec4 fillColor;

            vec4 surface()
            {
                return vec4(fillColor.rgb, 1);
            }
        ");

        public override RenderResult Render(RenderArgs args)
        {
            args.SurfaceB.Bind(FramebufferTarget.Framebuffer);
            _shader.Uniform4(_shader.GetUniformLocation("fillColor"), Color.GetValueAtTime(args.Time));
            GraphicsApi.DrawSurface(Matrix4.Identity, args.SurfaceA, _shader);
            return new RenderResult(true);
        }

        protected override ParameterList InitParameters() => new ParameterList(
            new NamedParameter("Color", Color)
        );

        public Fill()
        {
            Color.CustomUI = new NoAlphaColor4UI();
        }
    }
}
