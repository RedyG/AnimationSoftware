using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    internal class CommandHistory : IWindow
    {
        public string Name => "Command History";

        public void Show()
        {
            if (ImGui.Begin("Command History"))
            {
                foreach (var redo in CommandManager.RedoGroups.Reverse())
                {
                    ImGui.TextColored(Colors.Blue, redo.Name);
                }
                foreach (var undo in CommandManager.UndoGroups)
                {
                    ImGui.Text(undo.Name);
                }
            }
            ImGui.End();
        }
    }
}
