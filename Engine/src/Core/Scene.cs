using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public SizeF Size { get; set; }
        public float AspectRatio => Size.GetAspectRatio();


        public Scene(float frameRate, SizeF size, Timecode duration)
        {
            FrameRate = frameRate;
            Size = size;
            Duration = duration;
        }
    }
}