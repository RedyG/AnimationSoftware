using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public struct ContentEffectArgs
    {
        public SKSurface Surface { get; set; }
        public SKSize Size { get; set; }

        public ContentEffectArgs(SKSurface surface, SKSize size)
        {
            Surface = surface;
            Size = size;
        }
    }
}
