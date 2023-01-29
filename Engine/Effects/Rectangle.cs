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
        public Parameter<SKColor> Color { get; set; }
        public Parameter<SKPoint> Position { get; set; }
        public Parameter<SKSize> Size { get; set; }
        public Parameter<bool> FitToLayer { get; set; }
        public override void Render(SKSurface surface)
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
