using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class FilterEffect : Effect
    {
        public abstract SKShader MakeShader(SKShader input, float[] layerSize);
    }
}
