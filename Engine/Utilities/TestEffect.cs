using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core;

namespace Engine.Utilities
{
    public class UtilityDouble : Effect
    {
        public StaticProperty<float> Double { get; } = new();
        public KeyframableProperty<float> In { get; } = new();

        public override void Update(Timecode time)
        {
            Double.Value = In.GetValueAtTime(time) * 2;
        }
    }
}
