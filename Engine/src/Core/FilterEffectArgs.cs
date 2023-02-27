using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public readonly struct FilterEffectArgs
    {
        public SKShader InputShader { get; }
        public float[] Size { get;  }

        public FilterEffectArgs(SKShader inputShader, float[] size)
        {
            InputShader = inputShader;
            Size = size;
        }
    }
}
