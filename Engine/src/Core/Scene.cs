using Engine.Utilities;
using Newtonsoft.Json;
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
        // TODO: might wanna make this a LayerList and move all methods and properties like "SelectedLayers" inside the class.
        public LayerList Layers { get; } = new();

        public Timecode Duration { get; set; }
        public float FrameRate { get; set; }
        public Size Size { get; set; }
        public Color4 BackgroundColor { get; set; } = Color4.White;

        [JsonIgnore]
        public float AspectRatio => Size.GetAspectRatio();


        public Scene(float frameRate, Size size, Timecode duration)
        {
            FrameRate = frameRate;
            Size = size;
            Duration = duration;
        }
    }
}