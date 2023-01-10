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
        public List<Layer> Layers { get; private init; } = new List<Layer>();
        public Guid Id { get; private init; } = Guid.NewGuid();

        public Scene(float frameRate)
        {
            FrameRate = frameRate;
        }
    }
}