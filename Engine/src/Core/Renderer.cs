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

        public static Texture RenderActiveScene()
        {
            _mainLayer.Layers = App.Project.ActiveScene.Layers;
            _mainLayer.Size.Value = App.Project.ActiveScene.Size;

            GL.Viewport(new Point(0, 0), PreviewSize);
            return RenderLayer(_mainLayer, _surfaceA, _surfaceB);
        }

        public static Texture RenderLayer(Layer layer, Surface surfaceA, Surface surfaceB)
        {
            // TODO: reuse groups surfaces
            // TODO: optimize if only content effects by drawing directly on surface or stuff like that
            Surface firstSurface = surfaceA;

            if (layer.IsGroup)
            {
                firstSurface = GetSurface(layer);
                firstSurface.Framebuffer.Bind(FramebufferTarget.Framebuffer);
                GraphicsApi.Clear(new Color4(1f, 0f, 0f, 1f));
                foreach (Layer childLayer in layer.Layers)
                {
                    Texture childTexture = RenderLayer(childLayer, surfaceA, surfaceB);

                    firstSurface.Framebuffer.Bind(FramebufferTarget.Framebuffer);
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

                    GraphicsApi.DrawTexture(MatrixBuilder.CreateTransform(layer.Size.Value, childLayer), childTexture);
                }
            }

            bool swapSurfaces = false;
            foreach (Effect effect in layer.Effects)
            {
                Surface activeSurface = swapSurfaces ? surfaceB: firstSurface;
                Surface secondSurface = swapSurfaces ? firstSurface : surfaceB;

                activeSurface.Framebuffer.Bind(FramebufferTarget.Framebuffer);
                //GL.Viewport(layer.Size.Value.ToSize());
                var result = effect.Render(activeSurface, secondSurface, layer.Size.Value);
                if (result.SwapSurfaces)
                    swapSurfaces = !swapSurfaces;
            }


            return swapSurfaces ? surfaceB.Texture : firstSurface.Texture;
        }

        private static int MaxDepth(Layer layer)
        {
            if (!layer.IsGroup)
                return 0;

            int maxDepth = 0;
            foreach (Layer childLayer in layer.Layers)
                maxDepth = Math.Max(maxDepth, MaxDepth(childLayer));

            return maxDepth + 1;
        }
    }
}
