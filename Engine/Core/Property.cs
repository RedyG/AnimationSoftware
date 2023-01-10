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
    public class Property<T>
    {
        public KeyframeList<T> Keyframes { get; private init; } = new();
        public T GetValueAtTime(Timecode time)
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
    }
}