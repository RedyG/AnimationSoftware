using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public static class Renderer
    {
        public static void Preview()
        {

            if (App.Project == null || App.Project.ActiveScene == null) throw new Exception("Scene or project is null");

            // TODO: if an update is based on another this will give bad results -- https://stackoverflow.com/questions/5667816/get-types-used-inside-a-c-sharp-method-body
            foreach (var layer in App.Project.ActiveScene.Layers)
            {
                foreach (Effect effect in layer.Effects)
                {
                    effect.Update(App.Project.ActiveScene.Time);
                }
            }

            foreach (var layer in App.Project.ActiveScene.Layers)
            {
                foreach (Effect effect in layer.Effects)
                {
                    effect.Render(App.Project.ActiveScene.Time);
                }
            }

            App.Project.ActiveScene.Time.Frames++;
        }
    }
}
