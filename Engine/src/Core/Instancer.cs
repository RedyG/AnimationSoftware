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

namespace Engine.Core
{
    public static class Instancer
    {
        // optimize this with IL or Expression if it becomes an issue ( it won't )
        public static object? Create(Type type)
        {
            try
            {
                var activator = Activator.CreateInstance(type);
                return activator!;
            }
            catch (Exception ex)
            {
                return FormatterServices.GetUninitializedObject(type);
            }
        }

    }
}
