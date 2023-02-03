using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public static class Renderer
    {
        private static SKSurface _offScreenSurface;
        public static void Render()
        {
            for (App.Project!.ActiveScene!.Time.Frames = 0; App.Project.ActiveScene.Time >= App.Project.ActiveScene.Duration; App.Project.ActiveScene.Time.Frames++)
            {
                //RenderFrame();
                // Export Frame
            }
            // Convert to video using FFMPEG
        }

        public static void RenderScene(Scene scene, SKSurface surface)
        {
            var canvas = surface.Canvas;
            // TODO: make this line better
            // TODO: fix potential bug if user tries to render layer from another scene, it might not fit into the _offScreenSurface 
            _offScreenSurface = SKSurface.Create(surface.Context, false, new SKImageInfo((int)surface.Canvas.LocalClipBounds.Width - 2, (int)surface.Canvas.LocalClipBounds.Height - 2));
            foreach (var layer in scene.Layers)
            {
                ResizeCanvas(canvas, layer.Bounds);
                RenderLayer(layer, surface);
            }
        }
        public static void RenderLayer(Layer layer, SKSurface surface)
        {
            var canvas = surface.Canvas;
            _offScreenSurface.Canvas.Clear();

            if (layer.IsGroup)
            {
                foreach (Layer childLayer in layer.Layers) 
                {
                    ResizeCanvas(canvas, childLayer.Bounds);
                    RenderLayer(childLayer, surface);
                }
            }
            else
            {
                foreach (ContentEffect contentEffect in layer.ContentEffects)
                {
                    contentEffect.Render(_offScreenSurface!, layer.Size.Value);
                }
            }

            SKShader shader = _offScreenSurface!.Snapshot(new SKRectI(0, 0, (int)layer.Size.Value.Width, (int)layer.Size.Value.Height)).ToShader();
            foreach (FilterEffect filterEffect in layer.FilterEffects)
            {
                shader = filterEffect.MakeShader(shader, new float[2] {layer.Size.Value.Width, layer.Size.Value.Height});
            }

            using var paint = new SKPaint()
            {
                Shader = shader
            };

            canvas.DrawPaint(paint);
        }
        private static void ResizeCanvas(SKCanvas canvas, SKRect bounds)
        {
            canvas.Translate(bounds.Location);
            canvas.ClipRect(new SKRect(0, 0, bounds.Width, bounds.Height));
        }
    }
}
