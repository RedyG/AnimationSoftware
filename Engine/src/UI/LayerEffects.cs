using Engine.Attributes;
using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        public static float ParameterWidth = 400f;

        public static void DrawParameters(ParameterList parameters, bool root = false)
        {
            foreach (UIParameter namedParameter in parameters)
            {
            }
        }

        public string Name => "Layer Effects";

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
            {
                var firstPoint = ImGui.GetCursorScreenPos() - new Vector2(0f, ImGui.GetStyle().ItemSpacing.Y);
                var drawList = ImGui.GetWindowDrawList();
                drawList.AddRectFilled(firstPoint, firstPoint + new Vector2(ImGui.GetContentRegionAvail().X, effect.UIHeight), Colors.ToHex(ImGui.GetStyle().Colors[(int)ImGuiCol.PlotLinesHovered]));

                ImGui.Indent(ImGui.GetFrameHeight());
                effect.DrawParameters();
                effect.UIHeight = ImGui.GetCursorScreenPos().Y - firstPoint.Y;
                ImGui.Unindent(ImGui.GetFrameHeight());
            }

            ImGui.PopID();
        }

        public void Show()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0f, 0f));
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
                ImGui.DragFloat("Width", ref ParameterWidth);

                /*var foreground = ImGui.GetForegroundDrawList();
                foreground.AddLine(_linkStart, ImGui.GetMousePos(), Colors.BlueHex);*/
            }
            ImGui.End();
            ImGui.PopStyleVar();
        }
    }
}
