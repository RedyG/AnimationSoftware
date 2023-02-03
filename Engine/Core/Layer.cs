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
        // TODO: convert this to a parameter
        public SKRect Bounds => SKRect.Create(Position.Value, Size.Value);

        public Layer(SKPoint position, SKSize size)
        {
            Position = new(position);
            Size = new(size);
        }
    }
}
