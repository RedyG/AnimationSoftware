using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class FilterEffect : Effect
    {
        public abstract ShaderEffect MakeShaderEffect(ShaderEffect input);
        public abstract void UpdateUniforms(ShaderEffectProgram shaderEffectProgram);
    }
}
