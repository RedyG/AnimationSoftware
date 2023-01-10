using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public interface IEffect : ILayerItem
    {
        public void Render();
    }
}
