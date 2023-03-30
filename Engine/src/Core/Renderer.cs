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

        private static SizeF PreviewSizeF => App.Project.ActiveScene.Size * PreviewRatio;
        private static Size PreviewSize => new Size((int)(App.Project.ActiveScene.Size.Width * PreviewRatio), (int)(App.Project.ActiveScene.Size.Height * PreviewRatio));

        private static Texture testTexture = Texture.FromImage("Z:\\bliss brother.png", TextureTarget.Texture2D);

        private static Texture _textureA = Texture.Create(1920, 1080);
        private static Framebuffer _framebufferA = Framebuffer.FromTexture(_textureA);
        private static Surface _surfaceA = new(_textureA, _framebufferA);

        private static Texture _textureB = Texture.Create(1920, 1080);
        private static Framebuffer _framebufferB = Framebuffer.FromTexture(_textureB);
        private static Surface _surfaceB = new(_textureB, _framebufferB);


        private static Dictionary<Layer, Surface> _layerSurfaces = new Dictionary<Layer, Surface>();

        private static Surface GetSurface(Layer layer)
        {
            if (_layerSurfaces.TryGetValue(layer, out Surface surface))
                return surface;

            var texture = Texture.Create(App.Project.ActiveScene.Size.Width, App.Project.ActiveScene.Size.Height);
            var framebuffer = Framebuffer.FromTexture(texture);
            var newSurface = new Surface(texture, framebuffer);
            _layerSurfaces.Add(layer, newSurface);

            return newSurface;
        }


        private static Layer _mainLayer = new Layer("__mainLayer__", new(0f, 0f), new Size(0, 0));

        static Renderer()
        {
            _mainLayer.Effects.Add(new RenderChildren());
        }

        public static Texture RenderActiveScene()
        {
 
            _mainLayer.Layers = App.Project.ActiveScene.Layers;
            _mainLayer.Size.Value = App.Project.ActiveScene.Size;

            //GL.Viewport(new Point(0, 0), PreviewSize);
            return RenderLayer(new RenderArgs(App.Project.Time, _mainLayer, _surfaceA, _surfaceB));
        }

        public static Texture RenderLayer(RenderArgs args)
        {
            // TODO: reuse groups surfaces
            // TODO: optimize if only content effects by drawing directly on surface or stuff like that

            bool swapSurfaces = false;
            foreach (Effect effect in args.Layer.Effects)
            {
                Surface activeSurface = swapSurfaces ? args.SurfaceB : args.SurfaceA;
                Surface secondSurface = swapSurfaces ? args.SurfaceA : args.SurfaceB;
                activeSurface.Framebuffer.Bind(FramebufferTarget.Framebuffer);
                //GL.Viewport(layer.Size.Value.ToSize());
                var result = effect.Render(new RenderArgs(args.Time, args.Layer, activeSurface, secondSurface));
                if (result.SwapSurfaces)
                    swapSurfaces = !swapSurfaces;
            }

            return swapSurfaces ? args.SurfaceB.Texture : args.SurfaceA.Texture;
        }
    }
}
