using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class StaticProperty<T> : IProperty<T>
    {
        public T Value { get; set; }

        private IPropertyWrapper? _linkedProperty;
        public IPropertyWrapper? LinkedProperty
        {
            get => _linkedProperty;
            set
            {
                if (value == null)
                {
                    _linkedProperty = null;
                    return;
                }

                // TODO: emplement this in a better way
                if (App.Project == null || App.Project.ActiveScene == null) throw new Exception("Scene or project is null");

                try
                {
                    var temp = Reflection.GetValueAtTimeAsType<T>(value, App.Project.ActiveScene.Time);
                }
                catch
                {
                    throw new Exception("Couldn't cast.");
                }

                _linkedProperty = value;
            }
        }

        public T GetValueAtTime(Timecode time)
        {
            if (LinkedProperty == null)
                return Value;

            return Reflection.GetValueAtTimeAsType<T>(LinkedProperty, time);
        }
    }
}
