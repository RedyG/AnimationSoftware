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
                                for (int i = 0; i < keyframes.Count - 1; i++)
                                {
                                    var drawList = ImGui.GetWindowDrawList();
                                    var keyframe = keyframes[i];
                                    var nextKeyframe = keyframes[i + 1];
                                    Vector2 firstPoint = bottomLeft + new Vector2(keyframe.Time.Seconds * timeRatio, -parameter.GetEditorHeightAt(i));
                                    Vector2 secondPoint = bottomLeft + new Vector2(nextKeyframe.Time.Seconds * timeRatio, -parameter.GetEditorHeightAt(i + 1));
                                    keyframe.Easing.DrawUI(firstPoint, secondPoint);
                                    drawList.AddDiamondFilled(firstPoint, 3f, Colors.TextHex);
                                }
                            }
                        }
                    }
                }
            }
            ImGui.End();
        }
    }
}
