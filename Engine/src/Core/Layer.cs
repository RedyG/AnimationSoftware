﻿using Engine.Attributes;
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

        public string Name { get; set; }

        public Timecode StartTime = Timecode.FromSeconds(0);

        private Timecode _inPointOffset = Timecode.FromSeconds(0);
        public Timecode InPoint
        {
            get => _inPointOffset + StartTime;
            set => _inPointOffset = value - StartTime;
        }

        private Timecode _outPointOffset = App.Project.ActiveScene.Duration;
        public Timecode OutPoint
        {
            get => _outPointOffset + StartTime;
            set => _outPointOffset = value - StartTime;
        }

        public Timecode Duration
        {
            get => OutPoint - InPoint;
            set => OutPoint = value + InPoint;
        }

        public bool IsActiveAtTime(Timecode time)
        {
            return time >= InPoint && time < OutPoint;
        }

        [Param]
        public Parameter<PointF> Position { get; set; }

        [Param]
        public Parameter<PointF> Origin { get; set; } = new(new PointF(0f, 0f));

        [Param]
        public Parameter<SizeF> Size { get; set; }

        [Param]
        public Parameter<Vector2> Scale { get; set; } = new(new Vector2(1f));

        [Param]
        public Parameter<float> Rotation { get; set; } = new(0f);

        public Parameter<RectangleF> Bounds { get; set; } = new(RectangleF.Empty, false, false);


        public Layer(string name, PointF position, Size size)
        {
            Name = name;
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
