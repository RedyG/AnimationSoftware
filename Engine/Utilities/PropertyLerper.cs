using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities
{
    public static class PropertyLerper<T>
    {
        public delegate T LerpMethod(T value1, T value2, float amount);
        private static LerpMethod? method;

        public static T Lerp(T value1, T value2, float amount)
        {
            var value1PropetyValue = value1 as IPropertyValue<T>;

            if (value1PropetyValue == null)
            {
                if (method == null)
                    throw new Exception("Couldn't find the Lerp method on type " + typeof(T).FullName);
                else
                    return method(value1, value2, amount);
            }

            var value2PropertyValue = value2 as IPropertyValue<T>;
            var lerpMethod = typeof(T).GetMethod("Lerp");

            if (lerpMethod == null)
                throw new Exception("Couldn't find the Lerp method on type " + typeof(T).FullName);

            var result = lerpMethod.Invoke(value1, new object[] { value2!, amount });
            if (result == null)
                throw new Exception("The result of the Lerp method is null.");

            return (T)result;
        }

        public static void RegisterMethod(LerpMethod _method)
        {
            method = _method;
        }

        static PropertyLerper()
        {
            PropertyLerper<string>.RegisterMethod(
                (string value1, string value2, float time) => value1
            );
            PropertyLerper<float>.RegisterMethod(
                (float value1, float value2, float time) => (value1 + (value2 - value1) * time)
            );
            PropertyLerper<int>.RegisterMethod(
                (int value1, int value2, float time) => (int)(value1 + (value2 - value1) * time)
            );
            PropertyLerper<double>.RegisterMethod(
                (double value1, double value2, float time) => (double)(value1 + (value2 - value1) * time)
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