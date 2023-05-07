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
    [EffectDesc(Category = "Content")]
    public class Group : VideoEffect, IDisposable
    {
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
            if (!args.Layer.IsGroup)
                return new RenderResult(false);

            args.SurfaceB.Framebuffer.Bind(FramebufferTarget.Framebuffer);
            GraphicsApi.Clear(new Color4(0f, 0f, 0f, 1f));
            foreach (Layer childLayer in args.Layer.Layers)
            {
                Timecode childTime = args.Time - childLayer.Offset;
                if (!childLayer.IsActiveAtTime(args.Time))
                    continue;

                //Size layerSize = Renderer.ToPreviewSize(childLayer.Size.GetValueAtTime(args.Time));
                //var surfaceA = new Surface(args.SurfaceA.Texture, args.SurfaceA.Framebuffer, args.SurfaceA.Size, layerSize);
                //_groupSurface.Viewport = layerSize;
                Surface childSurface = Renderer.RenderLayer(new RenderArgs(childTime, childLayer, args.SurfaceA, _groupSurface));

                args.SurfaceB.Bind(FramebufferTarget.Framebuffer);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                GraphicsApi.DrawSurface(MatrixBuilder.CreateTransform(childTime, args.Layer.Transform.Size.GetValueAtTime(args.Time), childLayer), childSurface);
            }

            return new RenderResult(true);
        }

        protected override ParameterList InitParameters() => new ParameterList();
    }
}
