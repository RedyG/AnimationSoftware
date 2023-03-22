using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Layer
    {
        public List<Effect> Effects { get; set; } = new();
        public List<Layer> Layers { get; set; } = new();
        public bool IsGroup { get => Layers.Count > 0; }

        public Parameter<PointF> Position { get; set; }
        public Parameter<PointF> Origin { get; set; } = new(new PointF(0f, 0f));
        public Parameter<SizeF> Size { get; set; }
        public Parameter<RectangleF> Bounds { get; set; } = new(RectangleF.Empty, false, false);
        public Parameter<Vector2> Scale { get; set; } = new(new Vector2(1f));
        public Parameter<float> Rotation { get; set; } = new(0f);

        public Layer(PointF position, Size size)
        {
            Position = new(position);
            Size = new(size);

            Bounds.ValueSetter += (object? sender, ValueSetterEventArgs<RectangleF> args) =>
            {
                Position.SetValueAtTime(args.Time, args.Value.Location);
                Size.SetValueAtTime(args.Time, args.Value.Size);
            };
            Bounds.ValueGetter += (object? sender, ValueGetterEventArgs args) => new RectangleF(Position.GetValueAtTime(args.Time), Size.GetValueAtTime(args.Time));
        }
    }
}
