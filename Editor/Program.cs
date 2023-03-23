using Engine.src.Core;
using Engine.Utilities;
using System.Drawing;

namespace Editor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dict = new IndexedDictionary<string, int>();
            dict["pipi"] = 1;
            dict.Add(new("hey", 2333));
            dict.Add(new("allo", 233));
            dict.Add(new("bonjour", 23));
            dict["pipi"] = 2;

            Console.WriteLine(dict["pipi"]);
            Console.WriteLine(dict.Count);
            Console.WriteLine("start");

            foreach (var value in dict.Values)
            {
                Console.WriteLine(value);
            }
            //Window window = new Window("Editor", 1920, 1080);
            //window.Run();
        }
    }
}