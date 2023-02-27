using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Layer
    {
        public List<ContentEffect> ContentEffects { get; set; } = new();
        public List<FilterEffect> FilterEffects { get; set; } = new();
        public List<Layer> Layers { get; set; } = new();
        public bool IsGroup { get => Layers.Count > 0; }

        public Parameter<SKPoint> Position { get; set; }
        public Parameter<SKSize> Size { get; set; }
        public Parameter<SKRect> Bounds { get; set; } = new(SKRect.Empty, false, false);
        public Parameter<SKPoint> Scale { get; set; } = new(new SKPoint(1f, 1f));

        public Layer(SKPoint position, SKSize size)
        {
            Position = new(position);
            Size = new(size);

            Bounds.ValueSetter += (object? sender, ValueSetterEventArgs<SKRect> args) =>
            {
                Position.SetValueAtTime(args.Time, args.Value.Location);
                Size.SetValueAtTime(args.Time, args.Value.Size);
            };
            Bounds.ValueGetter += (object? sender, ValueGetterEventArgs args) => SKRect.Create(Position.GetValueAtTime(args.Time), Size.GetValueAtTime(args.Time));
        }
    }
}
