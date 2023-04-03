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


    public class Test : IParameterBehavior<float>
    {

        public void DrawUI(Parameter<float> parameter)
        {
            float value = parameter.Value;
            ImGui.DragFloat("", ref value);
            parameter.Value = value;
        }

        public float Lerp(float a, float b, float t)
        {
            return a;
        }
    }
}
