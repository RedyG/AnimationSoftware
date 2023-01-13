using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public interface IPropertyValue<T>
    {
        public T Lerp(T value2, float amount);
    }
}
