using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Scene
    {
        public List<Layer> Layers { get; } = new();


        public Timecode Time = Timecode.FromSeconds(0);
        public Timecode Duration { get; set; }
        public float FrameRate { get; set; }
        public SKSize Size { get; set; }


        public Scene(float frameRate, SKSize size, Timecode duration)
        {
            FrameRate = frameRate;
            Size = size;
            Duration = duration;
        }
    }
}