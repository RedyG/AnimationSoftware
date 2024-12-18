﻿using Engine.Attributes;
using Engine.UI;
using Engine.Utilities;
using ImGuiNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public readonly struct UIParameter
    {
        private static Parameter? _linkingParam = null;
        private static Parameter? _hoveredParam = null;

        public string? Path { get; } = null;
        public string Name { get; }
        public string FullName
        {
            get
            {
                if (Path == null)
                    return Name;

                return Path + "/" + Name;
            }
        }

        public Parameter Parameter { get; }

        public UIParameter(Parameter parameter, string name, string? path = null)
        {
            Path = path;
            Name = name;
            Parameter = parameter;
        }
        public void DrawUI()
        {
            ImGui.PushID(GetHashCode());

            float frameHeight = ImGui.GetFrameHeight();

            if (Parameter.UILocation == UILocation.Under)
            {
                bool opened = Parameter.Opened;

                ImGui.PushStyleColor(ImGuiCol.Button, Colors.Background);
                ImGuiHelper.ArrowButton("##opened", ref opened);
                ImGui.SameLine(0f, 0f);
                ImGui.PopStyleColor();

                Parameter.Opened = opened;
            }

            if (Parameter.CanBeKeyframed)
            {
                KeyframeButton();
            }
            ImGui.SameLine();

            Vector2 begRect = ImGui.GetCursorPos();
            float endRect = 0f;

            ImGui.Text(Name);

            if (Parameter.UILocation == UILocation.Right)
            {
                ImGui.SameLine();
                endRect = ImGui.GetContentRegionAvail().X - LayerEffects.ParameterWidth;
                ImGuiHelper.MoveCursorBy(endRect, 0);
                ImGui.SetNextItemWidth(LayerEffects.ParameterWidth);
                Parameter.DrawUI();
            }
            else
            {
                if (Parameter.Opened)
                {
                    ImGui.Indent(frameHeight);
                    endRect = ImGui.GetContentRegionAvail().X;
                    ImGui.SetNextItemWidth(endRect);
                    Parameter.DrawUI();
                    ImGui.Unindent(frameHeight);
                }
            }

            if (_hoveredParam == Parameter)
            {
                var drawList = ImGui.GetWindowDrawList();
                var lineHeight = ImGui.GetFrameHeight();
                drawList.AddRect(begRect, begRect + new Vector2(endRect, lineHeight), Colors.BlueHex);
            }

            if (_linkingParam != null && ImGui.IsItemHovered())
            {
                _hoveredParam = Parameter;

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    _linkingParam.LinkedParameter = Parameter;
            }

            if (ImGuiHelper.BeginContextPopup("Parameter Context"))
            {
                if (ImGui.MenuItem("Link to parameter"))
                {
                    _linkingParam = Parameter;
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();
        }
        private void KeyframeButton()
        {
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();
            var keyframed = Parameter.Keyframes!.IsKeyframedAtTime(App.Project.Time);
            Vector2 size = new(ImGui.GetFrameHeight());
            Vector2 center = cursorPos + new Vector2(size.X / 2);

            if (ImGui.InvisibleButton(Parameter.GetHashCode().ToString(), size))
            {
                if (keyframed)
                    Parameter.Keyframes.RemoveNearestAtTime(App.Project.Time);
                else
                    Parameter.Keyframes.Add(new(App.Project.Time, IEasing.Linear));
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                drawList.AddRectFilled(cursorPos, cursorPos + size, Colors.DarkGrayHoveredHex);
            }
            drawList.AddDiamondFilled(center, 5f, keyframed ? Colors.BlueHex : Colors.DarkGrayHex);
            drawList.AddDiamond(center, 5f, 1f, Colors.TextHex);
        }


        public static UIParameter Empty = new();
    }
}
