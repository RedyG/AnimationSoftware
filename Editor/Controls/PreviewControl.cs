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
            App.Project.ActiveScene = new Scene(30f, new SKSize(427, 365), Timecode.FromSeconds(0.1f));
            var layers = App.Project.ActiveScene.Layers;

            var groupLayer = new Layer(new SKPoint(0, 0), new SKSize(250, 250));
            groupLayer.FilterEffects.Add(new BlackAndWhite());
            layers.Add(groupLayer);

            var firstLayer = new Layer(new SKPoint(50, 50), new SKSize(100, 100));
            firstLayer.ContentEffects.Add(new Engine.Effects.Video());
            groupLayer.Layers.Add(firstLayer);
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var watch = new Stopwatch();
            watch.Start();
            Renderer.RenderScene(App.Project!.ActiveScene!, e.Surface);
            watch.Stop();
            Debug.WriteLine("abc " + watch.ElapsedMilliseconds);
        }
    }
}
