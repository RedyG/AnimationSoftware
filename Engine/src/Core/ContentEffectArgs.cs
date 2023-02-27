using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public readonly struct ContentEffectArgs
    {
        public SKSurface Surface { get; }
        public SKSize Size { get; }

        public ContentEffectArgs(SKSurface surface, SKSize size)
        {
            Surface = surface;
            Size = size;
        }
    }
}
