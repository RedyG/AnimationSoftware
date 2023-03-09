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

namespace Engine.Core
{
    public static class Renderer
    {
        public static float PreviewRatio { get; set; } = 1f;

        private static SizeF PreviewSizeF => App.Project.ActiveScene.Size * PreviewRatio;
        private static Size PreviewSize => new Size((int)(App.Project.ActiveScene.Size.Width * PreviewRatio), (int)(App.Project.ActiveScene.Size.Height * PreviewRatio));

        private static Texture _textureA = Texture.Create(1920, 1080);
        private static Framebuffer _framebufferA = Framebuffer.FromTexture(_textureA);
        private static Surface _surfaceA = new(_textureA, _framebufferA);

        private static Texture _textureB = Texture.Create(1920, 1080);
        private static Framebuffer _framebufferB = Framebuffer.FromTexture(_textureB);
        private static Surface _surfaceB = new(_textureB, _framebufferB);

        private static bool _surfacesSwapped = false;

        private static Surface _activeSurface => _surfacesSwapped ? _surfaceB : _surfaceA;
        private static Surface _secondSurface => _surfacesSwapped ? _surfaceA : _surfaceB;

        public static void RenderActiveScene(Surface surface)
        {
            GL.Viewport(new Point(0, 0), PreviewSize);

            GraphicsApi.Clear(App.Project.ActiveScene.BackgroundColor);

            foreach (Layer layer in App.Project.ActiveScene.Layers)
            {
                RenderLayer(layer, surface);
            }
        }

        public static void RenderLayer(Layer layer, Surface surface)
        {
            // TODO: reuse groups surfaces
            // TODO: optimize if only content effects by drawing directly on surface or stuff like that
            if (layer.IsGroup)
            {
                SizeF layerSize = layer.Size.Value;
                Texture texture = Texture.Create((int)layerSize.Width, (int)layerSize.Height);
                Framebuffer framebuffer = Framebuffer.FromTexture(texture);
                Surface groupSurface = new(texture, framebuffer);

                foreach (Layer childLayer in layer.Layers)
                {
                    RenderLayer(childLayer, groupSurface);
                }
            }

            _surfacesSwapped = false;
            foreach (Effect effect in layer.Effects)
            {
                _activeSurface.Framebuffer.Bind(FramebufferTarget.Framebuffer);
                var result = effect.Render(_activeSurface, _secondSurface);
                if (result.SwapSurfaces)
                    _surfacesSwapped = !_surfacesSwapped;
            }

            surface.Framebuffer.Bind(FramebufferTarget.Framebuffer);
            GraphicsApi.DrawTexture(MatrixBuilder.CreateTransform(layer), _activeSurface.Texture);
        }

        public static void AssertEnoughSurfaces(Scene scene)
        {
            // TODO: implement this for real
        }
    }
}
