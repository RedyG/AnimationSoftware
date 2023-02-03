using Engine.Core;
using SkiaSharp;

namespace Engine.Effects
{
    public class Fill : ContentEffect
    {
        public Parameter<SKColor> Color { get; set; } = new(new SKColor(255, 255, 255));

        public override void Render(SKSurface surface, SKSize layerSize)
        {
            var canvas = surface.Canvas;
            canvas.Clear(Color.Value);
        }

        public Fill() { }
        public Fill(SKColor color)
        {
            Color = new(color);
        }
    }
}
