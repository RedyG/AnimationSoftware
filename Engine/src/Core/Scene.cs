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
        // TODO: might wanna make this a LayerList and move all methods and properties like "SelectedLayers" inside the class.
        public List<Layer> Layers { get; } = new();

        public List<Layer> SelectedLayers
        {
            get
            {
                var result = new List<Layer>();
                GetSelectedLayers(Layers, result);
                return result;
            }
        }

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

        private static void GetSelectedLayers(List<Layer> layers, List<Layer> result)
        {
            foreach (var layer in layers)
            {
                if (layer.Selected)
                    result.Add(layer);

                if (layer.IsGroup)
                    GetSelectedLayers(layer.Layers, result);
            }
        }
    }
}