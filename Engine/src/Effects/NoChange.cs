using Engine.Core;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class NoChange : FilterEffect
    {
        private static string _src = @"
            uniform shader input;
            uniform float amount;

            half4 main(float2 coord) {
                return sample(input) / amount;
            }
        ";

        private static ShaderEffect _effect = new(_src);

        public override ShaderEffect MakeShaderEffect(ShaderEffect input)
        {
            _effect.Children["input"] = input;
            return _effect;
        }

        public override void UpdateUniforms(ShaderEffectProgram shaderEffectProgram)
        {
            shaderEffectProgram.Uniform1(_effect, "amount", 2f);
        }
    }
}
