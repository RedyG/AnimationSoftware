using Engine.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class Text : ContentEffect
    {
        public Parameter<string> Content { get; set; } = new("hello");
        public override void Render(SKSurface surface, SKSize layerSize)
        {
            var canvas = surface.Canvas;

            using var paint = new SKPaint()
            {
                TextSize = 20,
                Color = new SKColor(0, 0, 0)
            };

            canvas.DrawText(Content.Value, new SKPoint(0, -20), paint);
        }
    }
}
