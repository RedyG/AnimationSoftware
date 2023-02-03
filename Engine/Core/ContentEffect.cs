using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class ContentEffect : Effect
    {
        public abstract void Render(SKSurface surface, SKSize layerSize);
    }
}
