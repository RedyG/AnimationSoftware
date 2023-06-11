using Engine.Attributes;
using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    [OpenedByDefault]
    public class LayerEffects : IWindow
    {
        public static ImFontPtr BigFont;

        public static void KeyframeButton(Parameter parameter)
        {
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();
            var keyframed = parameter.IsKeyframedAtTime(App.Project.Time);

            if (ImGui.InvisibleButton(parameter.GetHashCode().ToString(), new(16, 16)))
            {
                if (keyframed)
                    parameter.RemoveNearestKeyframeAtTime(App.Project.Time);
                else
                    parameter.AddKeyframeAtTime(App.Project.Time, IEasing.Linear);
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                drawList.AddRectFilled(cursorPos, cursorPos + new Vector2(16, 16), Colors.DarkGrayHex);
            }
            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathLineTo(cursorPos + new Vector2(13, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 13));
            drawList.PathLineTo(cursorPos + new Vector2(3, 8));
            var color = Colors.BlueHex;
            drawList.PathFillConvex(keyframed ? color : Colors.DarkGrayHex);

            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathLineTo(cursorPos + new Vector2(13, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 13));
            drawList.PathLineTo(cursorPos + new Vector2(3, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathStroke(Colors.TextHex);
        }

        public static void DrawParameters(ParameterList parameters, bool root = false)
        {
            foreach (NamedParameter namedParameter in parameters)
            {
                ImGui.PushID(namedParameter.GetHashCode());

                if (namedParameter.Parameter.UILocation == UILocation.Under)
                {
                    bool opened = namedParameter.Parameter.Opened;
                    if (root)
                        ImGuiHelper.MoveCursorBy(4f, 0f);
                    ImGui.PushStyleColor(ImGuiCol.Button, Colors.Background);
                    ImGuiHelper.ArrowButton("##opened", ref opened);
                    ImGui.PopStyleColor();
                    ImGui.SameLine(0f, 4f);
                    namedParameter.Parameter.Opened = opened;
                }
                else
                {
                    ImGuiHelper.MoveCursorBy(ImGui.GetFrameHeight() + 8f, 0f);
                }

                KeyframeButton(namedParameter.Parameter);
                ImGui.SameLine();


                ImGui.Text(namedParameter.Name);

                if (namedParameter.Parameter.UILocation == UILocation.Right)
                {
                    ImGui.SameLine();
                    ImGuiHelper.MoveCursorBy(ImGui.GetContentRegionAvail().X - _parameterWidth, 0);
                    ImGui.SetNextItemWidth(_parameterWidth);
                    namedParameter.Parameter.DrawUI();
                }
                else
                {
                    if (namedParameter.Parameter.Opened)
                    {
                        ImGui.Indent(ImGui.GetFrameHeight() + 8f);
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                        namedParameter.Parameter.DrawUI();
                        ImGui.Unindent(ImGui.GetFrameHeight() + 8f);
                    }
                }
                ImGui.PopID();
            }
        }

        public string Name => "Layer Effects";

        private static float _parameterWidth = 400f;
        private List<Effect> _effectsToDelete = new();
        public void EffectUI(Effect effect)
        {
            ImGui.PushID(effect.GetHashCode());
            string name = effect.Description.Name;

            bool header = ImGui.CollapsingHeader(name);

            if (ImGuiHelper.BeginContextPopup("Effect Popup"))
            {
                if (ImGui.MenuItem("Delete"))
                    _effectsToDelete.Add(effect);

                ImGui.EndPopup();
            }

            if (header)
                DrawParameters(effect.Parameters, true);

            ImGui.PopID();
        }

        public void Show()
        {
            if (ImGui.Begin("Layer Effects"))
            {
                // TODO: only for selected layers
                var selectedLayers = App.Project.ActiveScene.Layers.Selected;
                foreach (Layer layer in selectedLayers)
                {
                    ImGui.PushFont(BigFont);
                    var drawList = ImGui.GetWindowDrawList();
                    drawList.AddRectFilled(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + ImGuiHelper.GetLineSize() - new Vector2(0f, 4f), Colors.MidGrayHex);
                    ImGuiHelper.TextCentered(layer.Name);
                    ImGui.PopFont();

                    ImGui.PushID(layer.GetHashCode());

                    ImGuiHelper.MoveCursorBy(4f, 0f);
                    ImGuiHelper.TextCentered("Miscellaneous");

                    EffectUI(layer.Transform);

                    _effectsToDelete.Clear();

                    foreach (Effect effect in layer.OtherEffects)
                        EffectUI(effect);

                    if (layer.VideoEffects.Count > 0)
                    {
                        ImGuiHelper.MoveCursorBy(4f, 0f);
                        ImGuiHelper.TextCentered("Video Effects");
                    }

                    foreach (VideoEffect effect in layer.VideoEffects)
                        EffectUI(effect);

                    foreach (Effect effect in _effectsToDelete)
                        layer.DeleteEffect(effect);

                    ImGui.PopID();
                }
                ImGuiHelper.MoveCursorBy(0, ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeight());
                ImGui.DragFloat("Width", ref _parameterWidth);
            }
            ImGui.End();
        }
    }
}
