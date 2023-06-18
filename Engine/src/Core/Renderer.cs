using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine.OpenGL;
using Engine.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Engine.Utilities;
using System.Drawing.Drawing2D;
using System.Security.AccessControl;
using Engine.Effects;

namespace Engine.Core
{
    public static class Renderer
    {
        public static float PreviewRatio { get; set; } = 1f;

        public static Size ToPreviewSize(Size size) => new Size((int)(size.Width * PreviewRatio), (int)(size.Height * PreviewRatio));
        public static Size ToPreviewSize(SizeF size) => new Size((int)(size.Width * PreviewRatio), (int)(size.Height * PreviewRatio));

        public static Size FromPreviewSize(Size size) => new Size((int)(size.Width / PreviewRatio), (int)(size.Height / PreviewRatio));
        public static Size FromPreviewSize(SizeF size) => new Size((int)(size.Width / PreviewRatio), (int)(size.Height / PreviewRatio));

        private static Texture testTexture = Texture.FromImage("Z:\\bliss brother.png", TextureTarget.Texture2D);

        private static Texture _textureA = Texture.Create(1920, 1080);
        private static Framebuffer _framebufferA = Framebuffer.FromTexture(_textureA);
        private static Surface _surfaceA = new(_textureA, _framebufferA, new Size(1920, 1080));

        private static Texture _textureB = Texture.Create(1920, 1080);
        private static Framebuffer _framebufferB = Framebuffer.FromTexture(_textureB);
        private static Surface _surfaceB = new(_textureB, _framebufferB, new Size(1920, 1080));


        private static Layer _mainLayer = new Layer("__mainLayer__", new(0f, 0f), new Size(0, 0));

        static Renderer()
        {
            var rectangle = new Effects.Rectangle();
            rectangle.Color.Value = Color4.Black;
            rectangle.FitToLayer.Value = true;
            _mainLayer.VideoEffects.Add(rectangle);
            _mainLayer.VideoEffects.Add(new Group());
        }

        public static Surface RenderActiveScene()
        {
 
            _mainLayer.Layers = App.Project.ActiveScene.Layers;
            _mainLayer.Transform.Size.Value = App.Project.ActiveScene.Size;

            //GL.Viewport(new Point(0, 0), PreviewSize);
            return RenderLayer(new RenderArgs(App.Project.Time, _mainLayer, _surfaceA, _surfaceB));
        }

        public static Surface RenderLayer(RenderArgs args)
        {
            //GL.Enable(EnableCap.Blend);
            //GL.Disable(EnableCap.DepthTest); //or glDepthMask(GL_FALSE)? depth tests break blending
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            // TODO: reuse groups surfaces
            // TODO: optimize if only content effects by drawing directly on surface or stuff like that
            Size size = ToPreviewSize(args.Layer.Transform.Size.GetValueAtTime(args.Time));
            args.SurfaceA.Viewport = size;
            args.SurfaceB.Viewport = size;


            bool swapSurfaces = false;
            foreach (VideoEffect effect in args.Layer.VideoEffects)
            {
                Surface activeSurface = swapSurfaces ? args.SurfaceB : args.SurfaceA;
                Surface secondSurface = swapSurfaces ? args.SurfaceA : args.SurfaceB;

                secondSurface.Framebuffer.Bind(FramebufferTarget.Framebuffer);
                GraphicsApi.Clear(new Color4(0f, 0f, 0f, 0f));

                activeSurface.Bind(FramebufferTarget.Framebuffer);

                var result = effect.Render(new RenderArgs(args.Time, args.Layer, activeSurface, secondSurface));
                if (result.SwapSurfaces)
                    swapSurfaces = !swapSurfaces;
            }

            return swapSurfaces ? args.SurfaceB : args.SurfaceA;
        }
    }
}
