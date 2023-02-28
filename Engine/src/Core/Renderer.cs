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
        private static Framebuffer _offscreenFramebuffer;

        public static void RenderScene(Scene scene, Framebuffer framebuffer)
        {
            foreach (Layer layer in scene.Layers)
            {
                RenderLayer(layer, framebuffer);
            }
        }

        public static void RenderLayer(Layer layer, Framebuffer framebuffer)
        {
            _offscreenFramebuffer.Bind(FramebufferTarget.Framebuffer);
            if (layer.IsGroup)
            {
                foreach (Layer childLayer in layer.Layers)
                {
                    RenderLayer(childLayer, framebuffer);
                }
            }
            else
            {
                foreach (ContentEffect contentEffect in layer.ContentEffects)
                {
                    //contentEffect.Render(new ContentEffectArgs())
                }
            }

            ShaderEffect shaderEffect = ShaderEffect.TextureEffect;
            foreach (FilterEffect filterEffect in layer.FilterEffects)
            {
                /*filterEffect.ShaderEffect.Children["input"] = shaderEffect;
                shaderEffect = filterEffect.ShaderEffect;*/
            }
            framebuffer.Bind(FramebufferTarget.Framebuffer);
        }
    }
}
