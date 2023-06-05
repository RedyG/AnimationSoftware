using Engine.Attributes;
using Engine.Core;
using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    public interface IWindow
    {
        public static List<AppWindow> Windows { get; } = new();

        // Todo: on plugin upload
        static IWindow()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IWindow).IsAssignableFrom(type))
                    {
                        if (type.IsAbstract || type.IsInterface)
                            continue;

                        IWindow window = (IWindow)Instancer.Create(type);
                        string name = StringUtilities.UnPascalCase(type.Name);
                        bool opened = type.GetCustomAttributes<OpenedByDefault>().Count() > 0;
                        Windows.Add(new AppWindow { Name = name, Opened = opened, Window = window });
                    }
                }
            }
        }

        string Name { get; }
        void Show();
    }

    public class AppWindow
    {
        public bool Opened;
        public string Name { get; init; }
        public IWindow Window { get; init; }

    }
}
