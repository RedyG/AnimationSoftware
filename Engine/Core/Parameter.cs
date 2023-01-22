using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class ParameterWrapper
    {
        public T GetValueAtTimeAsType<T>(Timecode time)
        {
            var result = GetType().GetMethod("GetValueAtTime")!.Invoke(this, new object[] { time })!;
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public void SetValueAtTimeAsType<T>(T value, Timecode time)
        {
            var castedValue = (T)Convert.ChangeType(value, typeof(T))!;
            GetType().GetMethod("SetValueAtTime")!.Invoke(this, new object[] { castedValue, time });
        }

        public abstract bool CanBeKeyframed { get; }
        public abstract bool IsKeyframed { get; }

    }
    public class Parameter<T> : ParameterWrapper
    {
        private KeyframeList<T> _keyframes = new();
        public KeyframeList<T>? Keyframes
        {
            get
            {
                if (CanBeKeyframed)
                {
                    return _keyframes;
                }

                return null;
            }
        }

        public override bool CanBeKeyframed { get; } = true;
        public override bool IsKeyframed
        {
            get
            {
                if (Keyframes == null)
                    return false;

                return Keyframes.Count != 0;
            }
        }
        private T _unKeyframedValue { get; set; }
        public T CurrentValue
        {
            get => GetValueAtTime(App.Project!.ActiveScene!.Time);
            set => SetValueAtTime(value, App.Project!.ActiveScene!.Time);
        }

        private ParameterWrapper? _linkedParameter;
        public ParameterWrapper? LinkedParameter
        {
            get => _linkedParameter;
            set
            {
                if (value == null)
                {
                    _linkedParameter = null;
                    return;
                }

                // TODO: emplement this in a better way
                if (App.Project == null || App.Project.ActiveScene == null) throw new Exception("Scene or project is null");

                try
                {
                    var temp = value.GetValueAtTimeAsType<T>(App.Project.ActiveScene.Time);
                }
                catch
                {
                    throw new Exception("Couldn't cast.");
                }

                _linkedParameter = value;
            }
        }

        public T GetValueAtTime(Timecode time)
        {
            if (LinkedParameter != null)
                return LinkedParameter.GetValueAtTimeAsType<T>(time);

            if (!IsKeyframed)
                return _unKeyframedValue;

            // TODO : optimize with binary search or something
            for (int i = 0; i < Keyframes!.Count - 1; i++)
            {
                var firstKeyframe = Keyframes[i];
                var secondKeyframe = Keyframes[i + 1];
                if (secondKeyframe.Time > time)
                {
                    var timeBetweenKeyframes = MathF.Max(MathUtilities.Map(time.Seconds, firstKeyframe.Time.Seconds, secondKeyframe.Time.Seconds, 0, 1), 0f);
                    var easedTime = firstKeyframe.Easing.Evaluate(timeBetweenKeyframes);
                    return ParameterValues<T>.Lerp(firstKeyframe.Value, secondKeyframe.Value, easedTime);
                }
            }
            return Keyframes[Keyframes.Count - 1].Value;
        }
        public void SetValueAtTime(T value, Timecode time)
        {
            if (IsKeyframed)
            {
                Keyframes!.Add(new Keyframe<T>(App.Project!.ActiveScene!.Time, value, EasingPresets.Linear));
            }
            else
            {
                _unKeyframedValue = value;
            }
        }

        public Parameter(T value)
        {
            _unKeyframedValue = value;
        }
        public Parameter(T value, bool canBeKeframed)
        {
            _unKeyframedValue = value;
            CanBeKeyframed = canBeKeframed;
        }
    }
}
