using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Keyframe<T>
    {
        internal Func<T, T>? ValidateMethod { get; set; }

        private Timecode _time;
        public Timecode Time
        {
            get => _time;
            set
            {
                _time = value;
                OnTimeChanged(EventArgs.Empty);
            }
        }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (ValidateMethod == null)
                    _value = value;
                else
                    _value = ValidateMethod(value);
            }
        }
        public IEasing Easing { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Keyframe(Timecode time, T value, IEasing easing)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        { 
            Value = value;
            Time = time;
            Easing = easing;
        }

        public event EventHandler<EventArgs>? TimeChanged;
        private void OnTimeChanged(EventArgs e)
        {
            if (TimeChanged != null)
                TimeChanged(this, e);
        }
    }
}
