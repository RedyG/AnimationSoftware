using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using OpenTK.Mathematics;

namespace Engine.Core
{
    public static class ParameterTypes
    {
        public static void Init()
        {
            Parameter<float>.RegisterType(
                (a, b, t) => Lerp(a, b, t),
                (ref float value) => ImGui.DragFloat("", ref value)
            );
            Parameter<int>.RegisterType(
                (a, b, t) => Convert.ToInt32(Lerp(a, b, t)),
                (ref int value) => ImGui.DragInt("", ref value)
            );

            Parameter<System.Numerics.Vector2>.RegisterType(
                (a, b, t) => new System.Numerics.Vector2(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t)),
                (ref System.Numerics.Vector2 value) =>
                {
                    ImGui.DragFloat2("", ref value);
                }
            );
            Parameter<PointF>.RegisterType(
                (a, b, t) => new PointF(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t)),
                (ref PointF value) =>
                {
                    var vec = value.ToVector2();
                    ImGui.DragFloat2("", ref vec);
                    value = new PointF(vec.X, vec.Y);
                }
            );
            Parameter<SizeF>.RegisterType(
                (a, b, t) => new SizeF(Lerp(a.Width, b.Width, t), Lerp(a.Height, b.Height, t)),
                (ref SizeF value) =>
                {
                    var vec = value.ToVector2();
                    ImGui.DragFloat2("", ref vec);
                    value = new SizeF(vec.X, vec.Y);
                }
            );
            Parameter<Color4>.RegisterType(
                (a, b, t) => new Color4(Lerp(a.R, b.R, t), Lerp(a.G, b.G, t), Lerp(a.B, b.B, t), Lerp(a.A, b.A, t)),
                (ref Color4 value) =>
                {
                    var vec = new System.Numerics.Vector4(value.R, value.G, value.B, value.A);
                    //ImGui.DragFloat4("", ref vec);
                    ImGui.ColorEdit4("", ref vec);
                    value = new Color4(vec.X, vec.Y, vec.Z, vec.W);
                }
            );

            Parameter<bool>.RegisterType(
                (a, b, t) => a,
                (ref bool value) => ImGui.Checkbox("", ref value)
            );
            Parameter<string>.RegisterType(
                (a, b, t) => a,
                (ref string value) => ImGui.Text(value)
            );
        }

        public static float Lerp(float a, float b, float t) => a * (1f - t) + b * t;
    }
}
