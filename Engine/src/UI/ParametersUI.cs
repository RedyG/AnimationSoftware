using Engine.Core;
using Engine.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    public class StringUI : IParameterUI<string>
    {
        public void Draw(Parameter<string> parameter)
        {
            var value = parameter.Value;
            ImGui.InputTextMultiline("", ref value, 99999, new Vector2(ImGui.GetContentRegionAvail().X, 400));
            parameter.Value = value;
        }
    }
    public class FloatUI : IParameterUI<float>
    {
        public float Speed { get; set; } = 1f;
        public float Minimum { get; set; } = float.MinValue;
        public float Maximum { get; set; } = float.MaxValue;

        public void Draw(Parameter<float> parameter)
        {
            var value = parameter.Value;
            ImGui.DragFloat("", ref value, Speed, Minimum, Maximum);
            parameter.Value = value;
        }
    }
    public class IntUI : IParameterUI<int>
    {
        public int Speed { get; set; } = 1;
        public int Minimum { get; set; } = int.MinValue;
        public int Maximum { get; set; } = int.MaxValue;

        public void Draw(Parameter<int> parameter)
        {
            var value = parameter.Value;
            ImGui.DragInt("", ref value, Speed, Minimum, Maximum);
            parameter.Value = value;
        }
    }
    public class PointFUI : IParameterUI<PointF>
    {
        public float Speed { get; set; } = 1f;
        public float Minimum { get; set; } = float.MinValue;
        public float Maximum { get; set; } = float.MaxValue;

        public void Draw(Parameter<PointF> parameter)
        {
            var vec = parameter.Value.ToVector2();
            ImGui.DragFloat2("", ref vec, Speed, Minimum, Maximum);
            parameter.Value = new PointF(vec.X, vec.Y);
        }
    }
    public class SizeFUI : IParameterUI<SizeF>
    {
        public float Speed { get; set; } = 1f;
        public float Minimum { get; set; } = float.MinValue;
        public float Maximum { get; set; } = float.MaxValue;

        public void Draw(Parameter<SizeF> parameter)
        {
            var vec = parameter.Value.ToVector2();
            ImGui.DragFloat2("", ref vec, Speed, Minimum, Maximum);
            parameter.Value = new SizeF(vec.X, vec.Y);
        }
    }
    public class Vector2UI : IParameterUI<Vector2>
    {
        public float Speed { get; set; } = 1f;
        public float Minimum { get; set; } = float.MinValue;
        public float Maximum { get; set; } = float.MaxValue;

        public void Draw(Parameter<Vector2> parameter)
        {
            var vec = parameter.Value;
            ImGui.DragFloat2("", ref vec, Speed, Minimum, Maximum);
            parameter.Value = vec;
        }
    }
    public class Color4UI : IParameterUI<OpenTK.Mathematics.Color4>
    {
        public void Draw(Parameter<OpenTK.Mathematics.Color4> parameter)
        {
            var value = parameter.Value;
            var vec = new Vector4(value.R, value.G, value.B, value.A);
            ImGui.ColorEdit4("", ref vec);
            parameter.Value = new OpenTK.Mathematics.Color4(vec.X, vec.Y, vec.Z, vec.W);
        }
    }
    public class BoolUI : IParameterUI<bool>
    {
        public void Draw(Parameter<bool> parameter)
        {
            var value = parameter.Value;
            ImGui.Checkbox("", ref value);
            parameter.Value = value;
        }
    }
    public class RotationUI : IParameterUI<float>
    {
        public void Draw(Parameter<float> parameter)
        {
            var value = parameter.Value;
            ImGui.SliderAngle("", ref value);
            parameter.Value = value;
        }
    }
}
