using Engine.Core;
using Engine.Utilities;
using System.Drawing;

namespace Editor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParameterBehaviors.Init();
            var window = new Window("Test", 1920, 1080);
            window.Run();
        }
    }
}