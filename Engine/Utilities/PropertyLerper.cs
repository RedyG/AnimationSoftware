using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core;

namespace Engine.Utilities
{
    public static class PropertyLerper<T>
    {
        public delegate T LerpMethod(T value1, T value2, float amount);
        private static LerpMethod? method;

        public static T Lerp(T value1, T value2, float amount)
        {
            if (value1 is IPropertyValue<T> value1PropertyValue)
            {
                return value1PropertyValue.Lerp(value2, amount);
            }

            if (method == null)
                throw new Exception("Couldn't find the Lerp method on type " + typeof(T).FullName);

            return method(value1, value2, amount);
        }

        public static void RegisterMethod(LerpMethod _method)
        {
            method = _method;
        }

        static PropertyLerper()
        {
            PropertyLerper<string>.RegisterMethod(
                (string value1, string value2, float amount) => value1
            );
            PropertyLerper<float>.RegisterMethod(
                (float value1, float value2, float amount) => (value1 + (value2 - value1) * amount)
            );
            PropertyLerper<int>.RegisterMethod(
                (int value1, int value2, float amount) => (int)(value1 + (value2 - value1) * amount)
            );
            PropertyLerper<double>.RegisterMethod(
                (double value1, double value2, float amount) => (double)(value1 + (value2 - value1) * amount)
            );

            // TODO: register way more types
        }
    }

    public class CustomType : IPropertyValue<CustomType>
    {
        public CustomType Lerp(CustomType value2, float amount)
        {
            return this;
        }
    }
}