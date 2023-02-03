using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public struct FilterEffectArgs
    {
        public SKShader InputShader { get; set; }
        public float[] Size { get; set; }

        public FilterEffectArgs(SKShader inputShader, float[] size)
        {
            InputShader = inputShader;
            Size = size;
        }
    }
}
