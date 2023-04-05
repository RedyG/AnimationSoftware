using Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Engine.src.Core
{
    public static class Instancer
    {
        private static Dictionary<Type, Func<object>> _constructors = new();

        public static object Create(Type type)
        {
            if (_constructors.TryGetValue(type, out var constructor))
            {
                return constructor();
            }

            ConstructorInfo? typeConstructor = type.GetConstructor(Type.EmptyTypes);
            if (type.IsValueType || typeConstructor != null)
            {
                // using IL instead of Expression because IL is way faster at creating the delegate ( even though calling it is SLIGHTLY slower )
                var method = new DynamicMethod("EmitActivate", type, null, true);
                var generator = method.GetILGenerator();
                generator.Emit(OpCodes.Newobj, typeConstructor!);
                generator.Emit(OpCodes.Ret);
                var constructorDelegate = (Func<object>)method.CreateDelegate(typeof(Func<object>));

                _constructors.Add(type, constructorDelegate);
                return constructorDelegate();
            }

            // TODO: this is slow
            return FormatterServices.GetUninitializedObject(type);
        }

    }
}
