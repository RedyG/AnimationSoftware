using Engine.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class Rectangle : ContentEffect
    {
        public Parameter<SKColor> Color { get; set; } = new(new SKColor(255, 255, 255));
        public Parameter<SKPoint> Position { get; set; } = new(new SKPoint(0, 0));
        public Parameter<SKSize> Size { get; set; } = new(new SKSize(100, 100));
        public Parameter<bool> FitToLayer { get; set; } = new(true);
        public override void Render(SKSurface surface, SKSize layerSize)
        {
            var canvas = surface.Canvas;
            if (!FitToLayer.Value)
            {
                var position = Position.Value;
                var size = Size.Value;

                using (var paint = new SKPaint())
                {
                    canvas.DrawRect(position.X, position.X, size.Width, size.Height, paint); 
                }
            }
            else
                canvas.Clear(Color.Value);
        }
    }
}
