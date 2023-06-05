using Engine.Core;
using Engine.Utilities;
using FFMpegCore.Enums;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Engine.UI;
using System.Threading.Tasks;

namespace Engine.Core
{
    public static class ParameterBehaviors
    {
        public static float Lerp(float a, float b, float t) => a * (1f - t) + b * t;
        public static int Lerp(int a, int b, float t) => (int)MathF.Round(Lerp((float)a, (float)b, t));

        public static void Init()
        {
            Parameter<float>.DefaultTypeUI = typeof(FloatUI);
            Parameter<float>.DefaultTypeLerp = (a, b, t) => Lerp(a, b, t);
            Parameter<float>.EditorConverter = new FloatConverter();

            Parameter<int>.DefaultTypeUI = typeof(IntUI);
            Parameter<int>.DefaultTypeLerp = (a, b, t) => Lerp(a, b, t);

            Parameter<Vector2>.DefaultTypeUI = typeof(Vector2UI);
            Parameter<Vector2>.DefaultTypeLerp = (a, b, t) => new(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));

            Parameter<PointF>.DefaultTypeUI = typeof(PointFUI);
            Parameter<PointF>.DefaultTypeLerp = (a, b, t) => new(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));

            Parameter<SizeF>.DefaultTypeUI = typeof(SizeFUI);
            Parameter<SizeF>.DefaultTypeLerp = (a, b, t) => new(Lerp(a.Width, b.Width, t), Lerp(a.Height, b.Height, t));

            Parameter<OpenTK.Mathematics.Color4>.DefaultTypeUI = typeof(Color4UI);
            Parameter<OpenTK.Mathematics.Color4>.DefaultTypeLerp = (a, b, t) => new(Lerp(a.R, b.R, t), Lerp(a.G, b.G, t), Lerp(a.B, b.B, t), Lerp(a.A, b.A, t));

            Parameter<bool>.DefaultTypeUI = typeof(BoolUI);

            Parameter<string>.DefaultTypeUI = typeof(StringUI);

            Parameter<ParameterList>.DefaultTypeUI = typeof(ParameterListUI);
        }
    }
}
