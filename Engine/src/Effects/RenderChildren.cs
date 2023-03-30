using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class RenderChildren : Effect, IDisposable
    {
        private Surface _groupSurface;

        public RenderChildren()
        {
            Size size = App.Project.ActiveScene.Size;
            Texture texture = Texture.Create(size.Width, size.Height);
            Framebuffer framebuffer = Framebuffer.FromTexture(texture);
            _groupSurface = new Surface(texture, framebuffer);
        }

        public void Dispose()
        {
            _groupSurface.Texture.Dispose();
            _groupSurface.Framebuffer.Dispose();
        }

        public override RenderResult Render(RenderArgs args)
        {
            if (args.Layer.IsGroup)
            {
                args.SurfaceB.Framebuffer.Bind(FramebufferTarget.Framebuffer);
                GraphicsApi.Clear(new Color4(0f, 0f, 0f, 1f));
                foreach (Layer childLayer in args.Layer.Layers)
                {
                    Timecode childTime = args.Time - childLayer.StartTime;
                    if (!childLayer.IsActiveAtTime(args.Time))
                        continue;

                    Texture childTexture = Renderer.RenderLayer(new RenderArgs(childTime, childLayer, args.SurfaceA, _groupSurface));

                    args.SurfaceB.Framebuffer.Bind(FramebufferTarget.Framebuffer);
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

                    GraphicsApi.DrawTexture(MatrixBuilder.CreateTransform(childTime, args.Layer.Size.GetValueAtTime(args.Time), childLayer), childTexture);
                }
            }

            return new RenderResult(true);
        }
    }
}
