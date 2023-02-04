using SkiaSharp;
using Engine.Effects;
using SkiaSharp.Views.Desktop;
using Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Editor.Forms.Controls
{
    public class PreviewControl : SKGLControl
    {
        static PreviewControl()
        {
            App.Project = new Project("hey");
            App.Project.ActiveScene = new Scene(30f, new SKSize(1920, 1080), Timecode.FromSeconds(0.1f));
            var layers = App.Project.ActiveScene.Layers;

            var groupLayer = new Layer(new SKPoint(0, 0), new SKSize(1920, 1080));
            groupLayer.FilterEffects.Add(new BlackAndWhite());
            layers.Add(groupLayer);

            var firstLayer = new Layer(new SKPoint(0, 0), new SKSize(960, 1080));
            firstLayer.ContentEffects.Add(new Engine.Effects.Text());
            /*var rectEffect = new Engine.Effects.Rectangle()
            {
                FitToLayer = new(false),
                Position = new(new SKPoint(21.71f, 55.21f)),
                Size = new(new SKSize(19.18f, 105.14f)),
                Color = new(new SKColor(255, 0 , 0))
            };
            firstLayer.ContentEffects.Add(rectEffect);*/
            groupLayer.Layers.Add(firstLayer);
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var watch = new Stopwatch();
            watch.Start();
            Renderer.PreviewSize = new SKSizeI(Width, Height);
            Renderer.RenderScene(App.Project!.ActiveScene!, e.Surface);
            watch.Stop();
            Debug.WriteLine("abc " + watch.ElapsedMilliseconds);
        }
    }
}
