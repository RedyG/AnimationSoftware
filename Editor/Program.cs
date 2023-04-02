using Engine.Core;
using Engine.Utilities;
using System.Drawing;

namespace Editor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            App.Project = new Project("hey");
            App.Project.ActiveScene = new Scene(50f, new(0, 0), Timecode.FromSeconds(20));

            Parameter<PointF> param = new SplitableParameter<PointF>(new PointF(20f, 20f));
            var a = param.Value;

            UI.Window = new Window("Test", 1920, 1080);
            UI.Window.Run();
        }
    }
}