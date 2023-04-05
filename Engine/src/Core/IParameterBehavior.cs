using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public interface IParameterUI<T>
    {

        public void Draw(Parameter<T> parameter);
    }
}
