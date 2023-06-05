using Engine.Core;
using Engine.Utilities;
using Newtonsoft.Json;
using System.Drawing;

namespace Editor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParameterBehaviors.Init();

            var size = new Parameter<Size>(new Size(150, 100), false ,false);
            string json = JsonConvert.SerializeObject(size);
            var secondSize = JsonConvert.DeserializeObject<Parameter<Size>>(json);
            Console.WriteLine(secondSize);

            var window = new Window("Test", 1920, 1080);
            window.Run();
        }
    }
}