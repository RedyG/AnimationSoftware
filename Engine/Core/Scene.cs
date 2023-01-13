using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Scene
    {
        public float FrameRate { get; set; }
        public List<Layer> Layers { get; } = new();
        public Guid Id { get; } = Guid.NewGuid();

        public Timecode Time = Timecode.FromSeconds(0);

        public Scene(float frameRate)
        {
            FrameRate = frameRate;
        }
    }
}