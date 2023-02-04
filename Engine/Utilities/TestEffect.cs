using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core;
using Engine.Attributes;

namespace Engine.Utilities
{
    public class UtilityDouble : Effect
    {
        public Parameter<float> Double { get; } = new(20f, true, true);
        public Parameter<float> In { get; } = new(20f);

        public UtilityDouble()
        {
            Double.ValueGetter += (object sender, ValueGetterEventArgs args) => In.GetValueAtTime(args.Time) * 2f;
        }
    }
}
