using Engine.Attributes;
using Engine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    public class TestUI : Effect
    {
        [Param] public Parameter<float> Intensity { get; } = new(29f);
        [Param] public Parameter<double> Something { get; } = new(29);
        [Param] public Parameter<string> Text { get; } = new(String.Empty, false, true);
        [BeginGroup("Some Group")]
        [BeginGroup("This is a test")]
        [Param] public Parameter<PointF> Position { get; } = new(PointF.Empty);
        [EndGroup]
        [Param("Intensity")] public Parameter<float> Intensity2 { get; } = new(20f);
        [EndGroup]
        [UIMethod] public void Test()
        {
            ImGuiHelper.TextCentered("Method");
        }
    }
}
