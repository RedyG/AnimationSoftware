using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    public static class MainMenuBar
    {
        public static void Draw()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Windows"))
                {
                    foreach (AppWindow appWindow in IWindow.Windows)
                    {
                        ImGui.PushID(appWindow.GetHashCode());

                        ImGui.Checkbox("", ref appWindow.Opened);
                        ImGui.SameLine(0f, 4f);
                        ImGui.Text(appWindow.Name);

                        ImGui.PopID();

                    }
                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("save"))
                    App.SaveProject();

                if (ImGui.MenuItem("debug"))
                {
                    Console.WriteLine(App.Project);
                }
                ImGui.EndMainMenuBar();
            }

            foreach (AppWindow appWindow in IWindow.Windows)
            {
                if (appWindow.Opened)
                    appWindow.Window.Show();
            }
        }
    }
}
