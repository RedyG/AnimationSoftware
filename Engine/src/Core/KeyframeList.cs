using Engine.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class KeyframeList<T> : ICollection<Keyframe<T>>
    {
        private Func<T, T>? _validateMethod { get; set; }

        [JsonProperty]
        private readonly UndoableList<Keyframe<T>> _list = new();
        private int GetClosest(int val1, int val2, Timecode time)
        {
            if (time - _list[val1].Time >= _list[val2].Time - time)
                return val2;
            else
                return val1;
        }
        private void SortList()
        {
            _list.Sort((a, b) => a.Time.Seconds.CompareTo(b.Time.Seconds));

            if (_list.Count <= 1)
                return;

            for (int i = 1; i < _list.Count; i++)
            {
                if (_list[i - 1].Time == _list[i].Time)
                    _list.RemoveAt(i - 1);
            }
        }

        public int NearestIndexAtTime(Timecode time)
        {

            int n = _list.Count;

            // Corner cases
            if (time <= _list[0].Time)
                return 0;
            if (time >= _list[n - 1].Time)
                return n - 1;

            // Doing binary search
            int i = 0, j = n, mid = 0;
            while (i < j)
            {
                mid = (i + j) / 2;

                if (_list[mid].Time == time)
                    return mid;

                // If time is less than list element, then search in left
                if (time < _list[mid].Time)
                {

                    // If time is greater than previous to mid, return closest of two
                    if (mid > 0 && time > _list[mid - 1].Time)
                        return GetClosest(mid - 1,
                                     mid, time);

                    // Repeat for left half
                    j = mid;
                }
                else
                {
                    if (mid < n - 1 && time < _list[mid + 1].Time)
                        return GetClosest(mid,
                             mid + 1, time);
                    i = mid + 1; // update i
                }
            }

            // Only single element left after search
            return mid;
        }
        public int IndexOf(Keyframe<T> keyframe)
        {
            return _list.IndexOf(keyframe);
        }
        public void RemoveNearestAtTime(Timecode time)
        {
            _list.RemoveAt(NearestIndexAtTime(time));
        }
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
        public Keyframe<T> NearestAtTime(Timecode time)
        {
            return _list[NearestIndexAtTime(time)];
        }
        public Keyframe<T> At(int index)
        {
            return _list.ElementAt(0);
        }
        public Keyframe<T> this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public void Add(Keyframe<T> newKeyframe)
        {
            newKeyframe.TimeChanged += new EventHandler<EventArgs>(Keyframe_TimeChanged);
            newKeyframe.ValidateMethod = _validateMethod;
            CommandManager.IgnoreStack.Push(true);
            newKeyframe.Value = newKeyframe.Value; // we do that so the validate method is used
            CommandManager.IgnoreStack.Pop();

            for (int i = 0; i < _list.Count; i++)
            {
                var keyframe = _list[i];

                if (keyframe.Time == newKeyframe.Time)
                {
                    keyframe.Value = newKeyframe.Value;
                    return;
                }
                
                if (keyframe.Time > newKeyframe.Time)
                {
                    _list.Insert(i, newKeyframe);
                    return;
                }
            }

            _list.Add(newKeyframe);
        }
        public void Clear()
        {
            _list.Clear();
        }
        public bool Contains(Keyframe<T> keyframe)
        {
            return _list.Contains(keyframe);
        }
        public void CopyTo(Keyframe<T>[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }
        public bool Remove(Keyframe<T> keyframe)
        {
            return _list.Remove(keyframe);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        public IEnumerator<Keyframe<T>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private void Keyframe_TimeChanged(object? sender, EventArgs e)
        {
            SortList();
        }

        public IEnumerable<Keyframe<T>> Selected
        {
            get
            {
                foreach (var keyframe in _list)
                {
                    if (keyframe.Selected)
                        yield return keyframe;
                }
            }
        }

        public KeyframeList()
        { 
        }

        public KeyframeList(Func<T, T> validateMethod)
        {
            _validateMethod = validateMethod;
        }
    }
}
