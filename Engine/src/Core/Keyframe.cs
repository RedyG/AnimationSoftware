using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Keyframe<T>
    {
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
        public T Value { get; set; }
        public IEasing Easing { get; set; }

        public Keyframe(Timecode time, T value, IEasing easing)
        { 
            Time = time;
            Value = value;
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
