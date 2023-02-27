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
        public Parameter<string> Content { get; set; } = new("Text");
        public override void Render(ContentEffectArgs args)
        {
            var canvas = args.Surface.Canvas;

            using var paint = new SKPaint()
            {
                TextSize = 100,
                Color = new SKColor(255, 0, 127),
                IsAntialias = true,
                //Shader = (new BlackAndWhite()).MakeShader(new FilterEffectArgs(null!, new float[2] {0, 0}))
            };

            canvas.DrawText(Content.Value, new SKPoint(50, 50), paint);
        }
    }
}
