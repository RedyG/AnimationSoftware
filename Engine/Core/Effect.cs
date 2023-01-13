using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Effect
    {
        public virtual void Render(Timecode time) { }
        public virtual void Update(Timecode time) { }
    }
}
