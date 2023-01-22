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
        public Parameter<float> Double { get; } = new(20f);
        public Parameter<float> In { get; } = new(20f);

        public override void Update(Timecode time)
        {
            Double.SetValueAtTime(In.GetValueAtTime(time) * 2f, time);
        }
    }
}
