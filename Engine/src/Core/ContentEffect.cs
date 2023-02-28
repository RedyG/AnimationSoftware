using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class ContentEffect : Effect
    {
        // TODO: might wanna switch back to ContentEffectArgs

        /// <summary>
        /// The framebuffer will always be bound. It will only be useful if you want to bind another frambuffer so you can bind this one back.
        /// </summary>
        public abstract void Render(Framebuffer framebuffer, SizeF size);
    }
}
