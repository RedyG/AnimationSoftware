using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core;

namespace Engine.Utilities
{
    public static class ParameterValues<T>
    {
        public delegate T LerpMethod(T value1, T value2, float amount);
        public static Type BlazorComponent
        {
            get
            {
                if (is IParameterValue<T> value1ParameterValue)
                {
                    return value1ParameterValue.Lerp(value2, amount);
                }

                if (_lerpMethod == null)
                    throw new Exception("Couldn't find the Lerp method on type " + typeof(T).FullName);

                return _lerpMethod(value1, value2, amount);
            }
        }

        private static LerpMethod? _lerpMethod;

        public static T Lerp(T value1, T value2, float amount)
        {
            if (value1 is IParameterValue<T> value1ParameterValue)
            {
                return value1ParameterValue.Lerp(value2, amount);
            }

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

    public class CustomType : IParameterValue<CustomType>
    {
        public CustomType Lerp(CustomType value2, float amount)
        {
            return this;
        }
    }
}