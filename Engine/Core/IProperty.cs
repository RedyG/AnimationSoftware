using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public interface IPropertyWrapper
    {
    }
    public interface IProperty<T> : IPropertyWrapper
    {
        public T GetValueAtTime(Timecode time);
        public IPropertyWrapper? LinkedProperty { get; set; }
    }
}
