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

            var groupLayer = new Layer(new SKPoint(50, 50), new SKSize(1920, 1080));
            groupLayer.FilterEffects.Add(new NoChange());
            App.Project.ActiveScene.Layers.Add(groupLayer);

            var firstLayer = new Layer(new SKPoint(0, 0), new SKSize(100, 100));
            firstLayer.ContentEffects.Add(new Fill(new SKColor(255, 0, 0)));
            groupLayer.Layers.Add(firstLayer);

            var secondLayer = new Layer(new SKPoint(110, 110), new SKSize(100, 100));
            secondLayer.ContentEffects.Add(new Fill(new SKColor(0, 255, 0)));
            groupLayer.Layers.Add(secondLayer);
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            Renderer.Render(e.Surface);
        }
    }
}
