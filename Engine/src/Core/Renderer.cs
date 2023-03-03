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

namespace Engine.Core
{
    public static class Renderer
    {
        private static List<Surface> _surfaces = new();

        private static Texture _textureA = Texture.Create(1920, 1080);
        private static Framebuffer _framebufferA = Framebuffer.FromTexture(_textureA);
        private static Surface _surfaceA = new(_textureA, _framebufferA);

        private static Texture _textureB = Texture.Create(1920, 1080);
        private static Framebuffer _framebufferB = Framebuffer.FromTexture(_textureB);
        private static Surface _surfaceB = new(_textureB, _framebufferB);

        private static bool _surfacesSwapped = false;

        private static Surface _activeSurface => _surfacesSwapped ? _surfaceB : _surfaceA;
        private static Surface _secondSurface => _surfacesSwapped ? _surfaceA : _surfaceB;

        public static void RenderScene(Scene scene, Surface surface)
        {
            //AssertEnoughSurfaces(scene);
            
            foreach (Layer layer in scene.Layers)
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
            GraphicsApi.DrawTexture(_activeSurface.Texture);
        }

        public static void AssertEnoughSurfaces(Scene scene)
        {
            // TODO: implement this for real

            // this is temporary
            Texture texture = Texture.Create((int)scene.Size.Width, (int)scene.Size.Height);
            Framebuffer framebuffer = Framebuffer.FromTexture(texture);
            _surfaces.Add(new Surface(texture, framebuffer));
        }
    }
}
