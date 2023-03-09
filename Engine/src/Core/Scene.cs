using Engine.Utilities;
using OpenTK.Mathematics;
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


        public Timecode Duration { get; set; }
        public float FrameRate { get; set; }
        public Size Size { get; set; }
        public Color4 BackgroundColor { get; set; } = Color4.White;

        public float AspectRatio => Size.GetAspectRatio();


        public Scene(float frameRate, Size size, Timecode duration)
        {
            FrameRate = frameRate;
            Size = size;
            Duration = duration;
        }
    }
}