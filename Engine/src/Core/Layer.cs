using Engine.Attributes;
using Engine.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Layer
    {
        public UndoableList<Effect> OtherEffects { get; set; } = new();
        public UndoableList<VideoEffect> VideoEffects { get; set; } = new();

        public IEnumerable<Effect> Effects
        {
            get
            {
                yield return Transform;
                foreach (Effect effect in OtherEffects)
                {
                    yield return effect;
                }
                foreach (VideoEffect videoEffect in VideoEffects)
                {
                    yield return videoEffect;
                }
            }
        }

        public LayerList Layers { get; set; } = new();
        public bool IsGroup { get => Layers.Count > 0; }
        public bool Selected { get; set; } = false;

        private string _name;
        public string Name
        {
            get => _name;
            set => CommandManager.ExecuteSetter("Layer name changed", _name, value, name => _name = name);
        }

        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set => CommandManager.ExecuteIfNeeded(_visible, value, new VisibleCommand { Layer = this });
        }

        [JsonIgnore]
        public bool Opened = false;

        private Timecode _offset = Timecode.FromSeconds(0f);
        public Timecode Offset
        {
            get => _offset;
            set
            {
                Timecode diff = value - _offset;
                if (diff == Timecode.Zero)
                    return;

                CommandManager.Execute(new OffsetCommand(this, diff));
            }
        }

        public class OffsetCommand : ICommand
        {
            private Layer _layer;
            private Timecode _diff;
            public string Name => "Layer offset changed";

            public void Execute()
            {
                _layer._offset += _diff;
            }

            public void Undo()
            {
                _layer._offset -= _diff;
            }

            public OffsetCommand(Layer layer, Timecode diff)
            {
                _layer = layer;
                _diff = diff;
            }
        }

        private Timecode _inPoint = Timecode.FromSeconds(0);
        public Timecode InPoint
        {
            get => _inPoint + Offset;
            set => CommandManager.ExecuteSetter("Layer InPoint changed", _inPoint, value - Offset, newInPoint => _inPoint = newInPoint);
        }

        private Timecode _outPoint = App.Project.ActiveScene.Duration;
        public Timecode OutPoint
        {
            get => _outPoint + Offset;
            set => CommandManager.ExecuteSetter("Layer OutPoint changed", _outPoint, value - Offset, newOutPoint => _outPoint = newOutPoint);
        }

        public Timecode Duration
        {
            get => OutPoint - InPoint;
            set => OutPoint = value + InPoint;
        }

        public bool IsActiveAtTime(Timecode time) => time >= InPoint && time < OutPoint && Visible;

        public Transform Transform { get; set; }

        public Layer(string name, PointF position, Size size)
        {
            _name = name;
            Transform = new Transform(position, size);

            Transform.Scale.CustomUI = new Vector2UI { Speed = 0.01f };

            Transform.Bounds.ValueSetter += (object? sender, ValueSetterEventArgs<RectangleF> args) =>
            {
                Transform.Position.SetValueAtTime(args.Time, args.Value.Location);
                Transform.Size.SetValueAtTime(args.Time, args.Value.Size);
            };
            Transform.Bounds.ValueGetter += (object? sender, ValueGetterEventArgs args) => new RectangleF(Transform.Position.GetValueAtTime(args.Time), Transform.Size.GetValueAtTime(args.Time));

            Transform.PreviewSize.ValueSetter += (object? sender, ValueSetterEventArgs<Size> args) => Transform.Size.SetValueAtTime(args.Time, Renderer.FromPreviewSize(args.Value));
            Transform.PreviewSize.ValueGetter += (object? sender, ValueGetterEventArgs args) => Renderer.ToPreviewSize(Transform.Size.GetValueAtTime(args.Time));
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

        public bool DeleteEffect(Effect effect)
        {
            if (effect is VideoEffect videoEffect)
            {
                return VideoEffects.Remove(videoEffect);
            }

            return OtherEffects.Remove(effect);
        }

        private class VisibleCommand : ICommand
        {
            public Layer Layer;

            public string Name => "Layer visibility";

            public void Execute()
            {
                Layer._visible = !Layer._visible;
            }

            public void Undo()
            {
                Layer._visible = !Layer._visible;
            }
        }

        public class NameChangedCommand : ICommand
        {
            private Layer _layer;
            private string _newName;
            private string _oldName;

            public string Name => "Layer named changed";

            public void Execute()
            {
                _oldName = _layer._name;
                _layer._name = _newName;
            }

            public void Undo()
            {
                _layer._name = _oldName;
            }

            public NameChangedCommand(Layer layer, string newName)
            {
                _layer = layer;
                _newName = newName;
            }
        }
    }

    [Description(Hidden = true)]
    public class Transform : Effect
    {
        public Parameter<PointF> Position { get; }
        public Parameter<PointF> Origin { get; } = new(new PointF(0f, 0f));
        public Parameter<SizeF> Size { get; }
        public Parameter<Vector2> Scale { get; } = new(new Vector2(1f));
        public Parameter<float> Rotation { get; } = new(0f);

        [JsonIgnore]
        public Parameter<RectangleF> Bounds { get; } = new(RectangleF.Empty, false, false);
        [JsonIgnore]
        public Parameter<Size> PreviewSize { get; } = new(System.Drawing.Size.Empty, false, false);

        public Transform(PointF position, SizeF size)
        {
            Position = new(position);
            Size = new(size);
        }

        protected override ParameterList InitParameters() => new(
            new("Position", Position),
            new("Origin", Origin),
            new("Size", Size),
            new("Scale", Scale),
            new("Rotation", Rotation),
            new("Group", Parameter.CreateGroup(
                new ("a", Rotation),
                new ("b", Scale)
            ))
        );

    }
}
