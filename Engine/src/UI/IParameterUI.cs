using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    /// <summary>
    /// Defines a method that draws the UI of a given parameter using ImGui.
    /// </summary>
    /// <remarks>
    /// Any class class implementing this interface should have a default constructor because instances of this class will be dymatically generated.
    /// If a class doesn't have one, the instance will still be created but it will lead to worse performance.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IParameterUI<T>
    {
        public UILocation Location => UILocation.Right;
        public void Draw(Parameter<T> parameter);
    }
}
