using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public interface IParameterBehavior<T>
    {
        public T Lerp(T a, T b, float t);
        public void DrawUI(Parameter<T> parameter);
    }
}
