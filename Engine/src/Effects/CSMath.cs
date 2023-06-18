using Engine.Attributes;
using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class CSMath : Effect
    {
        private List<double> _numbers = new();
        private string _code = string.Empty;

        [UIMethod] public void Input()
        {
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 58f);
            ImGui.InputText("", ref _code, 9999);
            ImGui.SameLine();
            ImGui.Button("Compile");
        }
        [Param] public Parameter<double> Output = new(0f, false, false);
    }
}
