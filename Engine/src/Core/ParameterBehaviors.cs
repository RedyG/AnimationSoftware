using Engine.Core;
using Engine.Utilities;
using FFMpegCore.Enums;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public static class ParameterBehaviors
    {
        public static void Init()
        {
            Parameter<float>.DefaultTypeBehavior = new FloatBehavior();
            Parameter<int>.DefaultTypeBehavior = new IntBehavior();

            Parameter<Vector2>.DefaultTypeBehavior = new Vector2Behavior();
            Parameter<PointF>.DefaultTypeBehavior = new PointFBehavior();
            Parameter<SizeF>.DefaultTypeBehavior = new SizeFBehavior();
            Parameter<OpenTK.Mathematics.Color4>.DefaultTypeBehavior = new Color4Behavior();

            Parameter<bool>.DefaultTypeBehavior = new BoolBehavior();
            Parameter<string>.DefaultTypeBehavior = new StringBehavior();
        }
    }

    public class StringBehavior : IParameterBehavior<string>
    {
        public void DrawUI(Parameter<string> parameter)
        {
            var value = parameter.Value;
            ImGui.InputText("", ref value, uint.MaxValue);
            parameter.Value = value;
        }

        public string Lerp(string a, string b, float t) => a;
    }
    public class FloatBehavior : IParameterBehavior<float>
    {
        public float Speed { get; set; } = 1f;
        public float Minimum { get; set; } = float.MinValue;
        public float Maximum { get; set; } = float.MaxValue;

        public void DrawUI(Parameter<float> parameter)
        {
            var value = parameter.Value;
            ImGui.DragFloat("", ref value, Speed, Minimum, Maximum);
            parameter.Value = value;
        }

        public float Lerp(float a, float b, float t) => MathUtilities.Lerp(a, b, t);
    }
    public class IntBehavior : IParameterBehavior<int>
    {
        public int Speed { get; set; } = 1;
        public int Minimum { get; set; } = int.MinValue;
        public int Maximum { get; set; } = int.MaxValue;

        public void DrawUI(Parameter<int> parameter)
        {
            var value = parameter.Value;
            ImGui.DragInt("", ref value, Speed, Minimum, Maximum);
            parameter.Value = value;
        }

        public int Lerp(int a, int b, float t) => MathUtilities.Lerp(a, b, t);
    }
    public class PointFBehavior : IParameterBehavior<PointF>
    {
        public float Speed { get; set; } = 1f;
        public float Minimum { get; set; } = float.MinValue;
        public float Maximum { get; set; } = float.MaxValue;

        public void DrawUI(Parameter<PointF> parameter)
        {
            var vec = parameter.Value.ToVector2();
            ImGui.DragFloat2("", ref vec, Speed, Minimum, Maximum);
            parameter.Value = new PointF(vec.X, vec.Y);
        }

        public PointF Lerp(PointF a, PointF b, float t) => new PointF(MathUtilities.Lerp(a.X, b.X, t), MathUtilities.Lerp(a.Y, b.Y, t));
    }
    public class SizeFBehavior : IParameterBehavior<SizeF>
    {
        public float Speed { get; set; } = 1f;
        public float Minimum { get; set; } = float.MinValue;
        public float Maximum { get; set; } = float.MaxValue;

        public void DrawUI(Parameter<SizeF> parameter)
        {
            var vec = parameter.Value.ToVector2();
            ImGui.DragFloat2("", ref vec, Speed, Minimum, Maximum);
            parameter.Value = new SizeF(vec.X, vec.Y);
        }

        public SizeF Lerp(SizeF a, SizeF b, float t) => new SizeF(MathUtilities.Lerp(a.Width, b.Width, t), MathUtilities.Lerp(a.Height, b.Height, t));
    }
    public class Vector2Behavior : IParameterBehavior<Vector2>
    {
        public float Speed { get; set; } = 1f;
        public float Minimum { get; set; } = float.MinValue;
        public float Maximum { get; set; } = float.MaxValue;

        public void DrawUI(Parameter<Vector2> parameter)
        {
            var vec = parameter.Value;
            ImGui.DragFloat2("", ref vec, Speed, Minimum, Maximum);
            parameter.Value = vec;
        }

        public Vector2 Lerp(Vector2 a, Vector2 b, float t) => new Vector2(MathUtilities.Lerp(a.X, b.X, t), MathUtilities.Lerp(a.Y, b.Y, t));
    }
    public class Color4Behavior : IParameterBehavior<OpenTK.Mathematics.Color4>
    {
        public void DrawUI(Parameter<OpenTK.Mathematics.Color4> parameter)
        {
            var value = parameter.Value;
            var vec = new Vector4(value.R, value.G, value.B, value.A);
            //ImGui.DragFloat4("", ref vec);
            ImGui.ColorEdit4("", ref vec);
            parameter.Value = new OpenTK.Mathematics.Color4(vec.X, vec.Y, vec.Z, vec.W);
        }

        public OpenTK.Mathematics.Color4 Lerp(OpenTK.Mathematics.Color4 a, OpenTK.Mathematics.Color4 b, float t) =>
            new OpenTK.Mathematics.Color4(MathUtilities.Lerp(a.R, b.R, t), MathUtilities.Lerp(a.G, b.G, t), MathUtilities.Lerp(a.B, b.B, t), MathUtilities.Lerp(a.A, b.A, t));
    }
    public class BoolBehavior : IParameterBehavior<bool>
    {
        public void DrawUI(Parameter<bool> parameter)
        {
            var value = parameter.Value;
            ImGui.Checkbox("", ref value);
            parameter.Value = value;
        }

        public bool Lerp(bool a, bool b, float t) => a;
    }
}
