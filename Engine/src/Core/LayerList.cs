using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class LayerList : List<Layer>
    {
        public IEnumerable<Layer> SelectedEnumerable
        {
            get
            {
                foreach (var layer in this)
                {
                    if (layer.Selected)
                        yield return layer;

                    if (layer.IsGroup)
                        foreach (var childLayer in layer.Layers.SelectedEnumerable)
                            yield return childLayer;
                }
            }
        }

        public IReadOnlyCollection<Layer> Selected
        {
            get
            {
                var result = new List<Layer>();
                GetSelectedLayers(this, result);
                return result.AsReadOnly();
            }
        }

        private static void GetSelectedLayers(LayerList layers, List<Layer> result)
        {
            foreach (var layer in layers)
            {
                if (layer.Selected)
                    result.Add(layer);

                if (layer.IsGroup)
                    GetSelectedLayers(layer.Layers, result);
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
