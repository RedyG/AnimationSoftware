using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public static class Parameters<T>
    {
        public delegate void UIMethod(T value);
        public delegate T LerpMethod(T value1, T value2, float amount);

        private static UIMethod? _drawUI;
        private static LerpMethod? _lerp;

        public static T Lerp(T value1, T value2, float amount)
        {
            if (_lerp == null)
                throw new Exception("Couldn't find the Lerp method on type " + typeof(T).FullName);

            return _lerp(value1, value2, amount);
        }

        public static void DrawUI(T value)
        {
            if (_drawUI == null)
                throw new Exception("Couldn't find the DrawUI method on type " + typeof(T).FullName);

            _drawUI(value);
        }

        public static void RegisterType(LerpMethod lerp, UIMethod drawUI)
        {
            _lerp = lerp;
            _drawUI = drawUI;
        }

        static Parameters()
        {
            Parameters<int>.RegisterType((value1, value2, amount) => value1 + Convert.ToInt32((value2 - value1) * amount), (value) => { });
            Parameters<float>.RegisterType((value1, value2, amount) => value1 + ((value2 - value1) * amount), (value) => { });
            Parameters<PointF>.RegisterType((value1, value2, amount) => new PointF(Parameters<float>.Lerp(value1.X, value2.X, amount), Parameters<float>.Lerp(value1.Y, value2.Y, amount)), (value) => { });
            Parameters<SizeF>.RegisterType((value1, value2, amount) => new SizeF(Parameters<float>.Lerp(value1.Width, value2.Width, amount), Parameters<float>.Lerp(value1.Height, value2.Height, amount)), (value) => { });
            Parameters<Vector2>.RegisterType((value1, value2, amount) => new Vector2(Parameters<float>.Lerp(value1.X, value2.X, amount), Parameters<float>.Lerp(value1.Y, value2.Y, amount)), (value) => { });
            /*ParameterValues<string>.RegisterMethod(
                (string value1, string value2, float amount) => value1
            );
            ParameterValues<float>.RegisterMethod(
                (float value1, float value2, float amount) => (value1 + (value2 - value1) * amount)
            );
            ParameterValues<int>.RegisterMethod(
                (int value1, int value2, float amount) => (int)(value1 + (value2 - value1) * amount)
            );
            ParameterValues<double>.RegisterMethod(
                (double value1, double value2, float amount) => (double)(value1 + (value2 - value1) * amount)
            );*/

            // TODO: register way more types
        }
    }
}