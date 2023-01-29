using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class Parameter
    {
        public T GetCurrentValueAsType<T>() => GetValueAtTimeAsType<T>(App.Project!.ActiveScene!.Time);
        public void SetCurrentValueAsType<T>(T value) => SetValueAtTimeAsType<T>(value, App.Project!.ActiveScene!.Time);
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

        public KeyframeList<T> GetKeyframesAsType<T>()
        {
            var result = GetType().GetProperty("Keyframes")!.GetValue(this)!;
            return (KeyframeList<T>)Convert.ChangeType(result, typeof(KeyframeList<T>));
        }


        public abstract bool CanBeKeyframed { get; }
        public abstract bool IsKeyframed { get; }
        public abstract bool CanBeLinked { get; init; }
        public abstract bool IsLinked { get; }

    }
    public class Parameter<T> : Parameter
    {
        private T _unkeyframedValue;
        public T Value
        {
            get => GetValueAtTime(App.Project!.ActiveScene!.Time);
            set => SetValueAtTime(App.Project!.ActiveScene!.Time, value);
        }

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

        public delegate T ValueRequestedEventHandler(object sender, ValueRequestedEventArgs args);
        public event ValueRequestedEventHandler? ValueRequested;

        private Parameter? _linkedParameter;
        public Parameter? LinkedParameter
        {
            get => _linkedParameter;
            set
            {
                if (!CanBeLinked)
                    return;

                if (value == null)
                {
                    _linkedParameter = null;
                    return;
                }

                try
                {
                    var temp = value.GetValueAtTimeAsType<T>(App.Project!.ActiveScene!.Time);
                }
                catch
                {
                    throw new Exception("Couldn't cast.");
                }

                _linkedParameter = value;
            }
        }

        private bool _canBeLinked = true;
        public override bool CanBeLinked
        {
            get
            {
                if (ValueRequested != null)
                    return false;

                return _canBeLinked;
            }
            init => _canBeLinked = value;
        }
        public override bool IsLinked => LinkedParameter != null;

        public T GetValueAtTime(Timecode time)
        {
            if (IsLinked)
                return LinkedParameter!.GetValueAtTimeAsType<T>(time);

            if (ValueRequested != null)
                return ValueRequested(this, new ValueRequestedEventArgs(time));

            if (!IsKeyframed)
                return _unkeyframedValue;

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
        public void SetValueAtTime(Timecode time, T value)
        {
            if (IsKeyframed)
            {
                Keyframes!.Add(new Keyframe<T>(App.Project!.ActiveScene!.Time, value, EasingPresets.Linear));
            }
            else
            {
                _unkeyframedValue = value;
            }
        }

        public Parameter(T value)
        {
            _unkeyframedValue = value;
        }
        public Parameter(T value, bool canBeKeframed, bool canBeLinked)
        {
            _unkeyframedValue = value;
            CanBeKeyframed = canBeKeframed;
            CanBeLinked = canBeLinked;
        }

        public static implicit operator T(Parameter<T> parameter) => parameter.Value;
        public static implicit operator Parameter<T>(T value) => new Parameter<T>(value);
    }
    public class ValueRequestedEventArgs
    {
        public Timecode Time { get; }

        public ValueRequestedEventArgs(Timecode time)
        {
            Time = time;
        }
    }

}
