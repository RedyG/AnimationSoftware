using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Engine.Utilities;

namespace Engine.Core
{
    public class KeyframableProperty<T> : IProperty<T>
    {
        public static void A()
        {
            if (App.Project == null)
                throw new Exception("Project is null");
            if (App.Project.ActiveScene == null)
                throw new Exception("Scene is null");
        }
        public KeyframeList<T> Keyframes { get; private init; } = new();

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
            {
                for (int i = 0; i < Keyframes.Count - 1; i++)
                {
                    var firstKeyframe = Keyframes[i];
                    var secondKeyframe = Keyframes[i + 1];
                    if (secondKeyframe.Time > time)
                    {
                        var timeBetweenKeyframes = MathF.Max(MathUtilities.Map(time.Seconds, firstKeyframe.Time.Seconds, secondKeyframe.Time.Seconds, 0, 1), 0f);
                        return PropertyLerper<T>.Lerp(firstKeyframe.Value, secondKeyframe.Value, timeBetweenKeyframes);
                    }
                }

                // Time 
                return Keyframes[Keyframes.Count - 1].Value;
            }

            // TODO: catch errors
            return Reflection.GetValueAtTimeAsType<T>(LinkedProperty, time);
        }
    }
}