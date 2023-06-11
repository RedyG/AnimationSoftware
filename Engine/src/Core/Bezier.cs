using Engine.UI;
using Engine.Utilities;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Core
{

    public class Bezier : IEasing
    {
        private static readonly PointF _p0 = new PointF(0f, 0f);
        private static readonly PointF _p3 = new PointF(1f, 1f);

        public PointF P0 => _p0;
        public PointF P1 { get; set; }
        public PointF P2 { get; set; }
        public PointF P3 => _p3;

        public float Evaluate(float x)
        {
            float t = FindTforX(x, P0.X, P1.X, P2.X, P3.X);
            return FindYForT(t, P0.Y, P1.Y, P2.Y, P3.Y);
        }

        private static Bezier? _hoveredEasing;
        private static Point _hoveredPoint;

        public void DrawUI(Vector2 first, Vector2 second)
        {
            var drawList = ImGui.GetWindowDrawList();
            Vector2 diff = second - first;
            Vector2 p1 = new Vector2(diff.X * P1.X, diff.Y * P1.Y) + first;
            Vector2 p2 = new Vector2(diff.X * P2.X, diff.Y * P2.Y) + first;
            drawList.AddBezierCubic(first, p1, p2, second, Colors.BlueHex, 1f);

            ImGui.SetCursorScreenPos(p1 - new Vector2(4f));
            ImGui.InvisibleButton($"Handle 1_" + GetHashCode(), new Vector2(8f));


            if (ImGui.IsItemHovered())
            {
                _hoveredEasing = this;
                _hoveredPoint = Point.P1;
            }

            drawList.AddLine(first, p1, Colors.WhiteHex);
            drawList.AddCircleFilled(p1, 4f, Colors.TextHex);

            ImGui.SetCursorScreenPos(p2 - new Vector2(4f));
            ImGui.InvisibleButton($"Handle 2_" + GetHashCode(), new Vector2(8f));


            if (ImGui.IsItemHovered())
            {
                _hoveredEasing = this;
                _hoveredPoint = Point.P2;
            }

            if (_hoveredEasing == this)
            {
                if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    Vector2 vec = (ImGui.GetMousePos() - first) / diff;
                    if (ImGuiHelper.IsKeyDown(Keys.LeftShift))
                    {
                        if (vec.Y > 0.5f)
                            vec.Y = 1f;
                        else
                            vec.Y = 0f;
                    }

                    if (_hoveredPoint == Point.P1)
                        P1 = new PointF(MathUtilities.MinMax(vec.X, 0f, 1f), vec.Y);
                    else
                        P2 = new PointF(MathUtilities.MinMax(vec.X, 0f, 1f), vec.Y);
                }
                else
                    _hoveredEasing = null;

            }

            drawList.AddLine(p2, second, Colors.WhiteHex);
            drawList.AddCircleFilled(p2, 4f, Colors.TextHex);
        }

        public Bezier()
        {
            P1 = new PointF(0.33f, 0f);
            P2 = new PointF(0.67f, 1f);
        }

        public Bezier(PointF p1, PointF p2)
        {
            P1 = p1;
            P2 = p2;
        }
        public Bezier(float p1x, float p1y, float p2x, float p2y)
        {
            P1 = new PointF(p1x, p1y);
            P2 = new PointF(p2x, p2y);
        }

        private enum Point
        {
            P1,
            P2
        }

        // I have no clue how any of this works
        private const double EPSILON = 1E-10;
        
        private static float FindYForT(float t, float p0y, float p1y, float p2y, float p3y) => MathF.Pow((1 - t), 3) * p0y + 3 * t * MathF.Pow((1 - t), 2) * p1y + 3 * (1 - t) * MathF.Pow(t, 2) * p2y + MathF.Pow(t, 3) * p3y;
        private static float FindTforX(float x, float pa, float pb, float pc, float pd)
        {
            float t = 0;
            float[] roots = FindRoots(x, pa, pb, pc, pd);
            if (roots.Length > 0)
            {
                foreach (float _t in roots)
                {
                    if (_t < 0 || _t > 1) continue;
                    t = _t;
                    break;
                }
            }
            return t;
        }
        private static bool Approximately(float n0, float n1)
        {
            return Math.Abs(n1 - n0) <= EPSILON;
        }
        private static float[] FindRoots(float x, float pa, float pb, float pc, float pd)
        {
            // Find the roots for a cubic polynomial with bernstein coefficients
            // {pa, pb, pc, pd}. The function will first convert those to the
            // standard polynomial coefficients, and then run through Cardano's
            // formula for finding the roots of a depressed cubic curve.

            float pa3 = 3 * pa,
                  pb3 = 3 * pb,
                  pc3 = 3 * pc,
                  a = -pa + pb3 - pc3 + pd,
                  b = pa3 - 2 * pb3 + pc3,
                  c = -pa3 + pb3,
                  d = pa - x;

            // Fun fact: any Bezier curve may (accidentally or on purpose)
            // perfectly model any lower order curve, so we want to test 
            // for that: lower order curves are much easier to root-find.
            if (Approximately(a, 0))
            {
                // this is not a cubic curve.
                if (Approximately(b, 0))
                {
                    // in fact, this is not a quadratic curve either.
                    if (Approximately(c, 0))
                    {
                        // in fact , there are no solutions.
                        return new float[] { };
                    }
                    // linear solution:
                    return new float[] { -d / c };
                }
                // quadratic solution:
                float _q = MathF.Sqrt(c * c - 4 * b * d),
                      b2 = 2 * b;

                return new float[] {
                    (_q - c) / b2,
                    (-c - _q) / b2
                };
            }

            // At this point, we know we need a cubic solution,
            // and the above a/b/c/d values were technically
            // a pre-optimized set because a might be zero and
            // that would cause the following divisions to error.

            b /= a;
            c /= a;
            d /= a;

            float b3 = b / 3,
                  p = (3 * c - b * b) / 3,
                  p3 = p / 3,
                  q = (2 * b * b * b - 9 * b * c + 27 * d) / 27,
                  q2 = q / 2,
                  discriminant = q2 * q2 + p3 * p3 * p3,
                  u1, v1;

            // case 1: three real roots, but finding them involves complex
            // maths. Since we don't have a complex data type, we use trig
            // instead, because complex numbers have nice geometric properties.
            if (discriminant < 0)
            {
                float mp3 = -p / 3,
                      r = MathF.Sqrt(mp3 * mp3 * mp3),
                      t = -q / (2 * r),
                      cosphi = t < -1 ? -1 : t > 1 ? 1 : t,
                      phi = MathF.Acos(cosphi),
                      crtr = MathF.Cbrt(r),
                      t1 = 2 * crtr;

                return new float[] {
                    t1 * MathF.Cos(phi / 3) - b3,
                    t1 * MathF.Cos((phi + MathF.Tau) / 3) - b3,
                    t1 * MathF.Cos((phi + 2 * MathF.Tau) / 3) - b3
                };
            }

            // case 2: three real roots, but two form a "float root",
            // and so will have the same resultant value. We only need
            // to return two values in this case.
            else if (discriminant == 0)
            {
                u1 = q2 < 0 ? MathF.Cbrt(-q2) : -MathF.Cbrt(q2);
                return new float[] {
                    2 * u1 - b3,
                    -u1 - b3
                };
            }

            // case 3: one real root, 2 complex roots. We don't care about
            // complex results so we just ignore those and directly compute
            // that single real root.
            else
            {
                float sd = MathF.Sqrt(discriminant);
                u1 = MathF.Cbrt(-q2 + sd);
                v1 = MathF.Cbrt(q2 + sd);
                return new float[] { u1 - v1 - b3 };
            }
        }
    }  
}
