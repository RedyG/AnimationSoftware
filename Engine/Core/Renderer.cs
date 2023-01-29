using SkiaSharp;
using System;
using System.Collections.Generic;
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
        /*Epublic static SKSurface? _renderSurface;
        public static SKSurface? RenderSurface
        {
            get => _renderSurface;
            set
            {
                if (value == null)
                {
                    _renderSurface = null;
                    _offScreenSurface = null;
                    return;
                }

                _renderSurface = value;
                SKSurface.Create(value.Context, false, value.);
            }
        }
        private static SKSurface? _offScreenSurface;*/
        public static async void Render(SKSurface surface)
        {
            for (App.Project!.ActiveScene!.Time.Frames = 0; App.Project.ActiveScene.Time >= App.Project.ActiveScene.Duration; App.Project.ActiveScene.Time.Frames++)
            {
                RenderFrame(surface);
                //await Task.Delay(1000);
                // Export Frame
            }
            // Convert to video using FFMPEG
        }
        public static void RenderFrame(SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.Clear(new SKColor(255, 255, 255));
            foreach (var layer in App.Project!.ActiveScene!.Layers)
            {
                var layerSurface = SKSurface.Create(surface.Context, false, new SKImageInfo(2000, 2000));
                RenderLayer(layer, surface, layerSurface);
            }
        }

        private static void RenderLayer(Layer layer, SKSurface mainSurface, SKSurface offScreenSurface)
        {
            // TODO: optimize if there is no duplictator on layer by drawing on SKPicture or by translating/clipping the canvas instead of creating a surface.
            var canvas = mainSurface.Canvas;
            using var layerSurface = SKSurface.Create(mainSurface.Context, false, new SKImageInfo((int)layer.Size.Value.Width, (int)layer.Size.Value.Height));

            if (layer.IsGroup)
            {
                foreach (Layer childLayer in layer.Layers) 
                {
                    RenderLayer(childLayer, layerSurface, offScreenSurface);
                }
            }
            else
            {
                foreach (ContentEffect contentEffect in layer.ContentEffects)
                {
                    contentEffect.Render(layerSurface);
                }
            }

            SKShader shader = layerSurface.Snapshot().ToShader();
            foreach (FilterEffect filterEffect in layer.FilterEffects)
            {
                shader = filterEffect.GetShader(shader);
            }

            using var paint = new SKPaint()
            {
                Shader = shader
            };

            canvas.DrawRect(layer.Bounds, paint);
        }
    }
}
