using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Layer
    {
        public List<ILayerItem> Items { get; private init; } = new();
    }
}
