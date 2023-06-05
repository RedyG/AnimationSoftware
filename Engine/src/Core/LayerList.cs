using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class LayerList : UndoableList<Layer>
    {
        public IEnumerable<Layer> Selected
        {
            get
            {
                foreach (var layer in this)
                {
                    if (layer.Selected)
                        yield return layer;

                    if (layer.IsGroup)
                        foreach (var childLayer in layer.Layers.Selected)
                            yield return childLayer;
                }
            }
        }


        public void Traverse(Action<Layer> action)
        {
            foreach (var layer in this)
            {
                action(layer);

                if (layer.IsGroup)
                    layer.Layers.Traverse(action);
            }
        }

        public void UnselectAll()
        {
            foreach (var layer in this)
            {
                layer.Selected = false;

                if (layer.IsGroup)
                    layer.Layers.UnselectAll();
            }
        }
    }
}
