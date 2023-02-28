﻿using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Parameter<PointF> Position { get; set; }
        public Parameter<SizeF> Size { get; set; }
        public Parameter<RectangleF> Bounds { get; set; } = new(RectangleF.Empty, false, false);
        public Parameter<PointF> Scale { get; set; } = new(new PointF(1f, 1f));

        public Layer(PointF position, SizeF size)
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
