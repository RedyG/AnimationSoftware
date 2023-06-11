using Engine.Attributes;
using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    [Description(Category = "Content")]
    public class Group : VideoEffect, IDisposable
    {
        private static ShaderProgram _discard = Surface.CompileShader(@"
            vec4 surface()
{
vec4 color = texture(input, uv);
if (color.a < 0.1)
    discard;

return color;
}
        ");
        private Surface _groupSurface;

        public Group()
        {
            Size size = App.Project.ActiveScene.Size;
            Texture texture = Texture.Create(size.Width, size.Height);
            Framebuffer framebuffer = Framebuffer.FromTexture(texture);
            _groupSurface = new Surface(texture, framebuffer, size);
        }

        public void Dispose()
        {
            _groupSurface.Texture.Dispose();
            _groupSurface.Framebuffer.Dispose();
        }

        public override RenderResult Render(RenderArgs args)
        {

            foreach (var childLayer in args.Layer.Layers.Reverse())
            {
                Timecode childTime = args.Time - childLayer.Offset;
                if (!childLayer.IsActiveAtTime(args.Time))
                    continue;

                Surface result = Renderer.RenderLayer(new RenderArgs(childTime, childLayer, args.SurfaceB, _groupSurface));
                args.SurfaceA.Bind(FramebufferTarget.Framebuffer);
                //GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                GraphicsApi.DrawSurface(MatrixBuilder.CreateTransform(childTime, args.Layer.Transform.Size.GetValueAtTime(args.Time), childLayer), result);
            }

            return new(false);
        }

        protected override ParameterList InitParameters() => new ParameterList();
    }
}
