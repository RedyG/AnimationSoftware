using Engine.Core;
using ImGuiNET;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public static class UI
    {
        public static void KeyframeIcon(Parameter parameter)
        {
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();
            var keyframed = parameter.IsKeyframedAtTime(App.Project.Time);

            if (ImGui.InvisibleButton(parameter.GetHashCode().ToString(), new(16, 16)))
            {
                if (keyframed)
                    parameter.RemoveNearestKeyframeAtTime(App.Project.Time);
                else
                    parameter.AddKeyframeAtTime(App.Project.Time);
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                drawList.AddRectFilled(cursorPos, cursorPos + new Vector2(16, 16), Color(ImGui.GetStyle().Colors[(int)ImGuiCol.Button]));
            }
            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathLineTo(cursorPos + new Vector2(13, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 13));
            drawList.PathLineTo(cursorPos + new Vector2(3, 8));
            var color = Color(ImGui.GetStyle().Colors[(int)ImGuiCol.CheckMark]);
            drawList.PathFillConvex(keyframed ? color : Color(ImGui.GetStyle().Colors[(int)ImGuiCol.Button]));

            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathLineTo(cursorPos + new Vector2(13, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 13));
            drawList.PathLineTo(cursorPos + new Vector2(3, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathStroke(Color(ImGui.GetStyle().Colors[(int)ImGuiCol.Text]));
        }

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

        public static uint Color(byte r, byte g, byte b, byte a) { uint ret = a; ret <<= 8; ret += b; ret <<= 8; ret += g; ret <<= 8; ret += r; return ret; }
        public static uint Color(Vector4 vector) => Color((byte)(vector.X * 255f), (byte)(vector.Y * 255f), (byte)(vector.Z * 255f), (byte)(vector.W * 255f));
    }
}
