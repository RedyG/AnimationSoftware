using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public static class ParameterValues<T>
    {
        public delegate T LerpMethod(T value1, T value2, float amount);
        private static LerpMethod? _lerpMethod;

        private static Type _blazorComponent;
        public static Type BlazorComponent
        {
            get;
            set;
        }

        public static T Lerp(T value1, T value2, float amount)
        {
            if (_lerpMethod == null)
                throw new Exception("Couldn't find the Lerp method on type " + typeof(T).FullName);

            return _lerpMethod(value1, value2, amount);
        }

        public static void RegisterType(LerpMethod lerpMethod, Type blazorComponent)
        {
            _lerpMethod = lerpMethod;
            BlazorComponent = blazorComponent;
        }

        static ParameterValues()
        {
            ParameterValues<int>.RegisterType((value1, value2, amount) => value1 + Convert.ToInt32((value2 - value1) * amount), typeof(BezierEasing));
            ParameterValues<float>.RegisterType((value1, value2, amount) => value1 + ((value2 - value1) * amount), typeof(BezierEasing));
            ParameterValues<PointF>.RegisterType((value1, value2, amount) => new PointF(ParameterValues<float>.Lerp(value1.X, value2.X, amount), ParameterValues<float>.Lerp(value1.Y, value2.Y, amount)), typeof(BezierEasing));
            ParameterValues<Vector2>.RegisterType((value1, value2, amount) => new Vector2(ParameterValues<float>.Lerp(value1.X, value2.X, amount), ParameterValues<float>.Lerp(value1.Y, value2.Y, amount)), typeof(BezierEasing));
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