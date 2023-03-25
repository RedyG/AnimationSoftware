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

            var list = new List<KeyValuePair<string, Parameter>>
            {
                new KeyValuePair<string, Parameter>("caca", new Parameter<float>(20f)),
                new KeyValuePair<string, Parameter>("pipi", new Parameter<float>(50f)),
                new KeyValuePair<string, Parameter>("okay", new Parameter<float>(1f))
            };


            Window window = new Window("Editor", 1920, 1080);
            window.Run();
        }
    }
}