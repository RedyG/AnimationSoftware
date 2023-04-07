using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    public static class ImGuiHelper
    {
        public static Vector2 GetLineSize() => new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetFrameHeight());
        public static bool IsKeyDown(Keys key) => ImGui.IsKeyDown((ImGuiKey)key);
        public static bool IsKeyPressed(Keys key) => ImGui.IsKeyPressed((ImGuiKey)key);
        public static bool IsKeyReleased(Keys key) => ImGui.IsKeyReleased((ImGuiKey)key);
        public static void MoveCursorBy(Vector2 translate)
        {
            var cursorPos = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos(cursorPos + translate);
        }
        public static void MoveCursorBy(float x, float y)
        {
            var cursorPos = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos(cursorPos + new Vector2(x, y));
        }
    }
}
