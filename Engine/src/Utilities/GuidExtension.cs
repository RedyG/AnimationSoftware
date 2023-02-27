using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.src.Utilities
{
    public static class GuidExtension
    {
        public static string ToShaderFormat(this Guid guid)
        {
            return guid.ToString().Replace('-', '_');
        }
    }
}
