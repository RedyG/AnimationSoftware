using Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.src.Effects
{
    public class Expression : Effect
    {
        public Parameter<string> code = new Parameter<string>()

        public override RenderResult Render(RenderArgs args)
        {
            return new RenderResult(false);
        }

        protected override ParameterList InitParameters()
        {
            throw new NotImplementedException();
        }

        public Expression()
        {

        }
    }
}
