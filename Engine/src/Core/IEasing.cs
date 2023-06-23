using Engine.Attributes;
using Engine.Effects;
using Engine.UI;
using Engine.Utilities;
using ImGuiNET;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public interface IEasing
    {
        public static readonly Hold Hold = new Hold();
        public static readonly Linear Linear = new Linear();
        public static readonly InQuad InQuad = new InQuad();
        public static readonly OutQuad OutQuad = new OutQuad();
        public static readonly InOutQuad InOutQuad = new InOutQuad();
        public static readonly InCubic InCubic = new InCubic();
        public static readonly OutCubic OutCubic = new OutCubic();
        public static readonly InOutCubic InOutCubic = new InOutCubic();
        public static readonly InQuart InQuart = new InQuart();
        public static readonly OutQuart OutQuart = new OutQuart();
        public static readonly InOutQuart InOutQuart = new InOutQuart();
        public static readonly InQuint InQuint = new InQuint();
        public static readonly OutQuint OutQuint = new OutQuint();
        public static readonly InOutQuint InOutQuint = new InOutQuint();
        public static readonly InSine InSine = new InSine();
        public static readonly OutSine OutSine = new OutSine();
        public static readonly InOutSine InOutSine = new InOutSine();
        public static readonly InExpo InExpo = new InExpo();
        public static readonly OutExpo OutExpo = new OutExpo();
        public static readonly InOutExpo InOutExpo = new InOutExpo();
        public static readonly InCirc InCirc = new InCirc();
        public static readonly OutCirc OutCirc = new OutCirc();
        public static readonly InOutCirc InOutCirc = new InOutCirc();
        public static readonly InElastic InElastic = new InElastic();
        public static readonly OutElastic OutElastic = new OutElastic();
        public static readonly InOutElastic InOutElastic = new InOutElastic();
        public static readonly InBack InBack = new InBack();
        public static readonly OutBack OutBack = new OutBack();
        public static readonly InOutBack InOutBack = new InOutBack();
        public static readonly OutBounce OutBounce = new OutBounce();
        public static readonly InBounce InBounce = new InBounce();
        public static readonly InOutBounce InOutBounce = new InOutBounce();

        public float Evaluate(float t);

        public void DrawUI(Vector2 first, Vector2 second)
        {
            var drawList = ImGui.GetWindowDrawList();
            float yDiff = second.Y - first.Y;
            float xDiff = second.X - first.X;
            int steps = (int)Math.Max(Math.Min(Math.Abs(xDiff), Math.Abs(yDiff)), 2f);
            float yMult = 1f / steps;
            float xStep = xDiff / steps;

            for (int i = 0; i < steps + 1; i++)
                drawList.PathLineTo(first + new Vector2(xStep * i, Evaluate(yMult * i) * yDiff));

            drawList.PathStroke(Colors.BlueHex);
        }

        public static List<EasingGroup> Easings { get; } = new();

        public static void LoadEasingsFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(IEasing)))
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    var description = type.GetCustomAttribute<Description>();

                    string name = description?.Name ?? StringUtilities.UnPascalCase(type.Name);
                    var easing = new EasingGroup(name, type);

                    if (description == null)
                    {
                        Easings.Add(easing);
                        continue;
                    }

                    if (description.Hidden)
                        continue;

                    if (description.Category == null)
                    {
                        Easings.Add(easing);
                        continue;
                    }

                    var group = Easings.Find(group => group.Name == description.Category);
                    if (group == null)
                    {
                        group = new EasingGroup(description.Category);
                        group.Easings = new();
                        Easings.Add(group);
                    }

                    group.Easings!.Add(easing);
                }
            }
        }
    }

    public class EasingGroup
    {
        public string Name { get; set; }
        public Type? Type { get; set; } = null;
        public List<EasingGroup>? Easings { get; set; } = null;

        public EasingGroup(string name)
        {
            Name = name;
        }

        public EasingGroup(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }


    public class Hold : IEasing
    {
        public float Evaluate(float t) => 0f;

        public void DrawUI(Vector2 first, Vector2 second)
        {
            var drawList = ImGui.GetWindowDrawList();
            drawList.PathLineTo(first);
            drawList.PathLineTo(new Vector2(second.X, first.Y));
            drawList.PathLineTo(second);
            drawList.PathStroke(Colors.BlueHex);
        }
    }
    public class Linear : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.Linear(t);
        public void DrawUI(Vector2 first, Vector2 second)
        {
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddLine(first, second, Colors.BlueHex);
        }
    }

    [Description(Category = "Quad", Name = "In")]
    public class InQuad : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InQuad(t);
    }

    [Description(Category = "Quad", Name = "Out")]
    public class OutQuad : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutQuad(t);
    }

    [Description(Category = "Quad", Name = "InOut")]
    public class InOutQuad : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutQuad(t);
    }

    [Description(Category = "Cubic", Name = "In")]
    public class InCubic : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InCubic(t);
    }

    [Description(Category = "Cubic", Name = "Out")]
    public class OutCubic : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutCubic(t);
    }

    [Description(Category = "Cubic", Name = "InOut")]
    public class InOutCubic : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutCubic(t);
    }

    [Description(Category = "Quart", Name = "In")]
    public class InQuart : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InQuart(t);
    }

    [Description(Category = "Quart", Name = "Out")]
    public class OutQuart : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutQuart(t);
    }

    [Description(Category = "Quart", Name = "InOut")]
    public class InOutQuart : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutQuart(t);
    }
    [Description(Category = "Quint", Name = "In")]
    public class InQuint : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InQuint(t);
    }
    [Description(Category = "Quint", Name = "Out")]
    public class OutQuint : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutQuint(t);
    }
    [Description(Category = "Quint", Name = "InOut")]
    public class InOutQuint : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutQuint(t);
    }
    [Description(Category = "Sine", Name = "In")]
    public class InSine : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InSine(t);
    }
    [Description(Category = "Sine", Name = "Out")]
    public class OutSine : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutSine(t);
    }
    [Description(Category = "Sine", Name = "InOut")]
    public class InOutSine : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutSine(t);
    }
    [Description(Category = "Expo", Name = "In")]
    public class InExpo : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InExpo(t);
    }
    [Description(Category = "Expo", Name = "Out")]
    public class OutExpo : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutExpo(t);
    }
    [Description(Category = "Expo", Name = "InOut")]
    public class InOutExpo : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutExpo(t);
    }
    [Description(Category = "Circ", Name = "In")]
    public class InCirc : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InCirc(t);
    }
    [Description(Category = "Circ", Name = "Out")]
    public class OutCirc : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutCirc(t);
    }
    [Description(Category = "Circ", Name = "InOut")]
    public class InOutCirc : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutCirc(t);
    }
    [Description(Category = "Elastic", Name = "In")]
    public class InElastic : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InElastic(t);
    }
    [Description(Category = "Elastic", Name = "Out")]
    public class OutElastic : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutElastic(t);
    }
    [Description(Category = "Elastic", Name = "InOut")]
    public class InOutElastic : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutElastic(t);
    }
    [Description(Category = "Back", Name = "In")]
    public class InBack : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InBack(t);
    }
    [Description(Category = "Back", Name = "Out")]
    public class OutBack : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutBack(t);
    }
    [Description(Category = "Back", Name = "InOut")]
    public class InOutBack : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutBack(t);
    }
    [Description(Category = "Bounce", Name = "In")]
    public class InBounce : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InBounce(t);
    }
    [Description(Category = "Bounce", Name = "Out")]
    public class OutBounce : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.OutBounce(t);
    }
    [Description(Category = "Bounce", Name = "InOut")]
    public class InOutBounce : IEasing
    {
        public float Evaluate(float t) => EasingFunctions.InOutBounce(t);
    }

    public static class EasingFunctions
    {
        public static float Linear(float t) => t;

        public static float InQuad(float t) => t * t;
        public static float OutQuad(float t) => 1 - InQuad(1 - t);
        public static float InOutQuad(float t)
        {
            if (t < 0.5) return InQuad(t * 2) / 2;
            return 1 - InQuad((1 - t) * 2) / 2;
        }

        public static float InCubic(float t) => t * t * t;
        public static float OutCubic(float t) => 1 - InCubic(1 - t);
        public static float InOutCubic(float t)
        {
            if (t < 0.5) return InCubic(t * 2) / 2;
            return 1 - InCubic((1 - t) * 2) / 2;
        }

        public static float InQuart(float t) => t * t * t * t;
        public static float OutQuart(float t) => 1 - InQuart(1 - t);
        public static float InOutQuart(float t)
        {
            if (t < 0.5) return InQuart(t * 2) / 2;
            return 1 - InQuart((1 - t) * 2) / 2;
        }

        public static float InQuint(float t) => t * t * t * t * t;
        public static float OutQuint(float t) => 1 - InQuint(1 - t);
        public static float InOutQuint(float t)
        {
            if (t < 0.5) return InQuint(t * 2) / 2;
            return 1 - InQuint((1 - t) * 2) / 2;
        }

        public static float InSine(float t) => (1f - MathF.Cos((t * MathF.PI) / 2f));
        public static float OutSine(float t) => (float)Math.Sin(t * Math.PI / 2);
        public static float InOutSine(float t) => (float)(Math.Cos(t * Math.PI) - 1) / -2;

        public static float InExpo(float t) => (float)Math.Pow(2, 10 * (t - 1));
        public static float OutExpo(float t) => 1 - InExpo(1 - t);
        public static float InOutExpo(float t)
        {
            if (t < 0.5) return InExpo(t * 2) / 2;
            return 1 - InExpo((1 - t) * 2) / 2;
        }

        public static float InCirc(float t) => -((float)Math.Sqrt(1 - t * t) - 1);
        public static float OutCirc(float t) => 1 - InCirc(1 - t);
        public static float InOutCirc(float t)
        {
            if (t < 0.5) return InCirc(t * 2) / 2;
            return 1 - InCirc((1 - t) * 2) / 2;
        }

        public static float InElastic(float t) => 1 - OutElastic(1 - t);
        public static float OutElastic(float t)
        {
            float p = 0.3f;
            return (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - p / 4) * (2 * Math.PI) / p) + 1;
        }
        public static float InOutElastic(float t)
        {
            if (t < 0.5) return InElastic(t * 2) / 2;
            return 1 - InElastic((1 - t) * 2) / 2;
        }

        public static float InBack(float t)
        {
            float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        }
        public static float OutBack(float t) => 1 - InBack(1 - t);
        public static float InOutBack(float t)
        {
            if (t < 0.5) return InBack(t * 2) / 2;
            return 1 - InBack((1 - t) * 2) / 2;
        }

        public static float InBounce(float t) => 1 - OutBounce(1 - t);
        public static float OutBounce(float t)
        {
            float div = 2.75f;
            float mult = 7.5625f;

            if (t < 1 / div)
            {
                return mult * t * t;
            }
            else if (t < 2 / div)
            {
                t -= 1.5f / div;
                return mult * t * t + 0.75f;
            }
            else if (t < 2.5 / div)
            {
                t -= 2.25f / div;
                return mult * t * t + 0.9375f;
            }
            else
            {
                t -= 2.625f / div;
                return mult * t * t + 0.984375f;
            }
        }
        public static float InOutBounce(float t)
        {
            if (t < 0.5) return InBounce(t * 2) / 2;
            return 1 - InBounce((1 - t) * 2) / 2;
        }
    }
}
