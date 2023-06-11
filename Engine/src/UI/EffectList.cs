using Engine.Attributes;
using Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    [OpenedByDefault]
    public class EffectList : IWindow
    {
        public string Name => "Effect List";

        public void Show()
        {
            if (ImGui.Begin("Effects List"))
            {
                foreach (var categories in Effect.Effects)
                {
                    if (ImGui.TreeNode(categories.Key))
                    {
                        foreach (var namedEffect in categories.Value)
                        {
                            if (ImGui.MenuItem(namedEffect.Key))
                            {
                                foreach (Layer layer in App.Project.ActiveScene.Layers.Selected)
                                {
                                    layer.AddEffect(Effect.Create(namedEffect.Value));
                                }
                            }
                        }
                        ImGui.TreePop();
                    }
                }
            }
            ImGui.End();
        }
    }
}
