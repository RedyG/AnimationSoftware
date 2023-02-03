using Engine.Core;
using FFMpegCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class Video : ContentEffect
    {
        SKBitmap bitmap = SKBitmap.Decode(@"C:\Users\minio\OneDrive\Images\Sans titre-1.png");
        public override void Render(ContentEffectArgs args)
        {
            var watch = new Stopwatch();
            watch.Start();
            var canvas = args.Surface.Canvas;

            canvas.DrawBitmap(bitmap.Resize(new SKImageInfo((int)args.Size.Width, (int)args.Size.Height), SKFilterQuality.Medium), new SKPoint(0, 0));
            watch.Stop();
            Debug.WriteLine("aa: " + watch.ElapsedMilliseconds);
        }
    }
}
