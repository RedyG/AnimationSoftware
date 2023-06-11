using Engine.Attributes;
using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    [OpenedByDefault]
    public class Easing : IWindow
    {
        public string Name => "Easing";

        private static readonly Vector2 _buttonSize = new Vector2(2, 2);

        private static void Easings(Parameter parameter, int index, List<EasingGroup> easings)
        {
            foreach (EasingGroup group in easings)
            {
                if (group.Easings == null)
                {
                    if (ImGui.MenuItem(group.Name))
                        parameter.SetEasingAt(index, (IEasing)Instancer.Create(group.Type!)!);
                }
                else
                {
                    if (ImGui.BeginMenu(group.Name))
                    {
                        Easings(parameter, index, group.Easings);

                        ImGui.EndMenu();
                    }
                }
            }
        }

        private static void KeyframeContextMenu(Parameter parameter, int index)
        {
            if (ImGuiHelper.BeginContextPopup("Keyframe Edit"))
            {
                if (ImGui.BeginMenu("Set Easing"))
                {
                    Easings(parameter, index, IEasing.Easings);

                    ImGui.EndMenu();
                }

                ImGui.EndPopup();
            }
        }

        private static void DrawKeyframe(Vector2 point)
        {
            var drawList = ImGui.GetWindowDrawList();
            ImGui.SetCursorScreenPos(point - new Vector2(4f));
            ImGui.InvisibleButton("Keyframe", new Vector2(8f));


            drawList.AddDiamondFilled(point, 4f, Colors.TextHex);
        }

        public void Show()
        {
            if (ImGui.Begin(Name))
            {
                Vector2 size = ImGui.GetContentRegionAvail();
                Vector2 bottomLeft = ImGui.GetCursorScreenPos() + new Vector2(0, size.Y);
                float timeRatio = size.X / App.Project.ActiveScene.Duration.Seconds;

                var selectedLayers = App.Project.ActiveScene.Layers.Selected.ToList();
                if (selectedLayers.Count == 1)
                {
                    var layer = selectedLayers[0];
                    foreach (Effect effect in layer.Effects)
                    {
                        foreach (NamedParameter namedParameter in effect.Parameters)
                        {
                            var parameter = namedParameter.Parameter;

                            if (parameter.IsKeyframed)
                            {
                                List<KeyframeDefinition> keyframes = parameter.KeyframeDefinitions.ToList();

                                Vector2 secondPoint = bottomLeft + new Vector2(keyframes[0].Time.Seconds * timeRatio, -parameter.GetEditorHeightAt(0));
                                for (int i = 0; i < keyframes.Count - 1; i++)
                                {
                                    ImGui.PushID(i);
                                    var keyframe = keyframes[i];
                                    var nextKeyframe = keyframes[i + 1];
                                    Vector2 firstPoint = secondPoint;
                                    secondPoint = bottomLeft + new Vector2(nextKeyframe.Time.Seconds * timeRatio, -parameter.GetEditorHeightAt(i + 1));
                                    keyframe.Easing.DrawUI(firstPoint, secondPoint);

                                    DrawKeyframe(firstPoint);
                                    KeyframeContextMenu(parameter, i);

                                    ImGui.PopID();
                                }
                                DrawKeyframe(secondPoint);
                                KeyframeContextMenu(parameter, keyframes.Count - 1);
                            }
                        }
                    }
                }
            }
            ImGui.End();
        }
    }
}
