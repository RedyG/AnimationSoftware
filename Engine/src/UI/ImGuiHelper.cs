using Engine.Core;
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

        public static bool Shortcut(Keys mainKey, params Keys[] otherKeys)
        {
            foreach (Keys key in otherKeys)
            {
                if (!IsKeyDown(key))
                    return false;
            }

            return IsKeyPressed(mainKey);
        }


        private static object _firstValue = new object();
        public static void EditValue<T>(object value, Action<T> setValue)
        {
            EditValue(value, setValue, ImGui.IsItemActivated(), ImGui.IsItemDeactivated());
        }
        public static void EditValue<T>(object value, Action<T> setValue, bool beginEdit, bool endEdit)
        {
            if (beginEdit)
            {
                _firstValue = value;
                CommandManager.IgnoreStack.Push(true);
            }
            if (endEdit)
            {
                setValue((T)_firstValue);
                CommandManager.IgnoreStack.Push(false);
                setValue((T)value);
                CommandManager.IgnoreStack.Pop();
                CommandManager.IgnoreStack.Pop();
            }
            setValue((T)value);
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

        public static void TextCentered(string text)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);
        }

        public static void TextCentered(string text, Vector4 color)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.TextColored(color, text);
        }

        public static void ArrowButton(string id, ref bool opened)
        {
            ImGuiDir arrowDir = opened ? ImGuiDir.Down : ImGuiDir.Right;
            ImGui.ArrowButton(id, arrowDir);
            if (ImGui.IsItemClicked())
                opened = !opened;
        }
    }

    public static class DrawListExtension
    {
        public static void AddDiamondFilled(this ImDrawListPtr drawList, Vector2 pos, float size, uint color)
        {
            drawList.PathLineTo(pos + new Vector2(0, -size));
            drawList.PathLineTo(pos + new Vector2(size, 0));
            drawList.PathLineTo(pos + new Vector2(0, size));
            drawList.PathLineTo(pos + new Vector2(-size, 0));
            drawList.PathFillConvex(color);
        }

        public static void AddDiamond(this ImDrawListPtr drawList, Vector2 pos, float size, float thickness, uint color)
        {
            drawList.PathLineTo(pos + new Vector2(0, -size));
            drawList.PathLineTo(pos + new Vector2(size, 0));
            drawList.PathLineTo(pos + new Vector2(0, size));
            drawList.PathLineTo(pos + new Vector2(-size, 0));
            drawList.PathStroke(color, ImDrawFlags.None, thickness);
        }
    }
}
