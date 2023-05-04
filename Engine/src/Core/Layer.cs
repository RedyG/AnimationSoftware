using Engine.Attributes;
using Engine.UI;
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
        public List<Effect> OtherEffects { get; set; } = new();
        public List<VideoEffect> VideoEffects { get; set; } = new();
        public LayerList Layers { get; set; } = new();
        public bool IsGroup { get => Layers.Count > 0; }
        public bool Selected { get; set; } = false;

        public string Name;

        public bool Visible = true;

        public bool Opened = false;

        public Timecode Offset = Timecode.FromSeconds(0);

        private Timecode _inPoint = Timecode.FromSeconds(0);
        public Timecode InPoint
        {
            get => _inPoint + Offset;
            set => _inPoint = value - Offset;
        }

        private Timecode _outPoint = App.Project.ActiveScene.Duration;
        public Timecode OutPoint
        {
            get => _outPoint + Offset;
            set => _outPoint = value - Offset;
        }

        public Timecode Duration
        {
            get => OutPoint - InPoint;
            set => OutPoint = value + InPoint;
        }

        public bool IsActiveAtTime(Timecode time) => time >= InPoint && time < OutPoint && Visible;

        public LayerSettings Settings { get; set; }

        public Layer(string name, PointF position, Size size)
        {
            Name = name;
            Settings = new LayerSettings(position, size);

            Settings.Scale.CustomUI = new Vector2UI { Speed = 0.01f };

            Settings.Bounds.ValueSetter += (object? sender, ValueSetterEventArgs<RectangleF> args) =>
            {
                Settings.Position.SetValueAtTime(args.Time, args.Value.Location);
                Settings.Size.SetValueAtTime(args.Time, args.Value.Size);
            };
            Settings.Bounds.ValueGetter += (object? sender, ValueGetterEventArgs args) => new RectangleF(Settings.Position.GetValueAtTime(args.Time), Settings.Size.GetValueAtTime(args.Time));

            Settings.PreviewSize.ValueSetter += (object? sender, ValueSetterEventArgs<Size> args) => Settings.Size.SetValueAtTime(args.Time, Renderer.FromPreviewSize(args.Value));
            Settings.PreviewSize.ValueGetter += (object? sender, ValueGetterEventArgs args) => Renderer.ToPreviewSize(Settings.Size.GetValueAtTime(args.Time));
        }

        public void AddEffect(Effect effect)
        {
            if (effect is VideoEffect videoEffect)
            {
                VideoEffects.Add(videoEffect);
            }
            else
            {
                OtherEffects.Add(effect);
            }
        }
    }

    public class LayerSettings : Effect
    {
        public Parameter<PointF> Position { get; }
        public Parameter<PointF> Origin { get; } = new(new PointF(0f, 0f));
        public Parameter<SizeF> Size { get; }
        public Parameter<Vector2> Scale { get; } = new(new Vector2(1f));
        public Parameter<float> Rotation { get; } = new(0f);
        public Parameter<RectangleF> Bounds { get; } = new(RectangleF.Empty, false, false);
        public Parameter<Size> PreviewSize { get; } = new(System.Drawing.Size.Empty, false, false);

        public LayerSettings(PointF position, SizeF size)
        {
            Position = new(position);
            Size = new(size);
        }

        protected override ParameterList InitParameters() => new(
            new("Position", Position),
            new("Origin", Origin),
            new("Size", Size),
            new("Scale", Scale),
            new("Rotation", Rotation)
        );

    }
}
