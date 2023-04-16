using Engine.Core;
using Engine.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class Expression : Effect
    {
        public Parameter<string> Code { get; } = new Parameter<string>("", false, true);
        public Parameter<object> Value { get; } = new Parameter<object>(new object(), false, false);
        public override RenderResult Render(RenderArgs args)
        {
            return new RenderResult(false);
        }

        protected override ParameterList InitParameters() => new ParameterList(
            new("Code", Code),
            new("Value", Value)
        );

        public Expression()
        {
            Code.CustomUI = new ExpressionCodeUI();
        }
    }

    public class ExpressionCodeUI: IParameterUI<string>
    {
        public void Draw(Parameter<string> parameter)
        {
            var value = parameter.BeginValueChange();
            ImGui.InputTextMultiline("", ref value, 99999, new Vector2(ImGui.GetContentRegionAvail().X, 400));
            parameter.EndValueChange(value);
        }

    }
}
