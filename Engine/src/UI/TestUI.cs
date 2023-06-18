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
        [Param] public Parameter<float> Intensity { get; } = new();
        [Param] public Parameter<double> Something { get; } = new();
        [Param] public Parameter<string> Text { get; } = new(String.Empty, false, true);
        [BeginGroup("Some Group")]
        [BeginGroup("This is a test")]
        [Param] public Parameter<PointF> Position { get; } = new();
        [EndGroup]
        [Param("Intensity")] public Parameter<float> Intensity2 { get; } = new();
        [EndGroup]
        [UIMethod] public void Test()
        {
            ImGuiHelper.TextCentered("Method");
        }
    }
}
