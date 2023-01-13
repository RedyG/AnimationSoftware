using Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities
{
    public static class Reflection
    {
        public static bool HasMethod(this object objectToCheck, string methodName)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName, flags) != null;
        }

        public static T GetValueAtTimeAsType<T>(IPropertyWrapper property, Timecode time)
        {
            var result = property.GetType().GetMethod("GetValueAtTime")!.Invoke(property, new object[] { time })!;

            return (T)Convert.ChangeType(result, typeof(T));
        }
    }
}
