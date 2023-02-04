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
        public static SKSizeI PreviewSize { get; set; }
        private static SKSurface _offScreenSurface;
        private static SKSize _previewRatio;

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
            canvas.Clear();
            // TODO: make this line better
            // TODO: fix potential bug if user tries to render layer from another scene, it might not fit into the _offScreenSurface and _previewRatio will be off
            _offScreenSurface = SKSurface.Create(surface.Context, false, new SKImageInfo(PreviewSize.Width, PreviewSize.Height));
            _previewRatio = new SKSize((float)PreviewSize.Width / scene.Size.Width, (float)PreviewSize.Height / scene.Size.Height);

            foreach (var layer in scene.Layers)
            {
                canvas.Resize(layer.Bounds.Value.ApplyRatio(_previewRatio));
                RenderLayer(layer, surface);
            }
        }
        public static void RenderLayer(Layer layer, SKSurface surface)
        {
            var canvas = surface.Canvas;
            
            _offScreenSurface.Canvas.Clear();

            var layerPreviewSize = layer.Size.Value.ApplyRatio(_previewRatio);

            if (layer.IsGroup)
            {
                foreach (Layer childLayer in layer.Layers) 
                {
                    canvas.Resize(childLayer.Bounds.Value.ApplyRatio(_previewRatio));
                    RenderLayer(childLayer, surface);
                }
            }
            else
            {
                foreach (ContentEffect contentEffect in layer.ContentEffects)
                {
                    contentEffect.Render(new ContentEffectArgs(_offScreenSurface, layerPreviewSize));
                }
            }

            SKShader shader = _offScreenSurface
                .Snapshot(new SKRectI(0, 0, (int)layerPreviewSize.Width, (int)layerPreviewSize.Height))
                .ToShader();

            foreach (FilterEffect filterEffect in layer.FilterEffects)
            {
                shader = filterEffect.MakeShader(new FilterEffectArgs(shader, new float[2] { layerPreviewSize.Width, layerPreviewSize.Height }));
            }

            using var paint = new SKPaint()
            {
                Shader = shader
            };

            canvas.DrawPaint(paint);
        }

        public static SKRect ApplyRatio(this SKRect rect, SKSize ratio)
        {
            return new SKRect(rect.Left * ratio.Width,
                              rect.Top * ratio.Height,
                              rect.Right * ratio.Width,
                              rect.Bottom * ratio.Height);
        }
        public static SKSize ApplyRatio(this SKSize size, SKSize ratio)
        {
            return new SKSize(size.Width * ratio.Width,
                              size.Height * ratio.Height);
        }
        public static void Resize(this SKCanvas canvas, SKRect bounds)
        {
            canvas.Translate(bounds.Location);
            canvas.ClipRect(new SKRect(0, 0, bounds.Width, bounds.Height));
        }
    }
}
