using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public static class Lerper<T>
    {
        public delegate T LerpMethod(T value1, T value2, float amount);

        public static LerpMethod? Lerp;

        static Lerper()
        {
            Parameter<int>.RegisterType((value1, value2, amount) => value1 + Convert.ToInt32((value2 - value1) * amount), (value) => { });
            Parameter<float>.RegisterType((value1, value2, amount) => value1 + ((value2 - value1) * amount), (value) => { });
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