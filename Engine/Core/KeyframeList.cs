using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class KeyframeList<T> : ICollection<Keyframe<T>>
    {
        private List<Keyframe<T>> list = new();
        private int GetClosest(int val1, int val2, Timecode time)
        {
            if (time - list[val1].Time >= list[val2].Time - time)
                return val2;
            else
                return val1;
        }
        private void SortList()
        {
            list.Sort((a, b) => a.Time.Seconds.CompareTo(b.Time.Seconds));

            // Remove duplicates inplace
            int index = list.Count - 1;
            while (index > 0)
            {
                if (list[index].Time == list[index - 1].Time)
                {
                    if (index < list.Count - 1)
                        (list[index], list[list.Count - 1]) = (list[list.Count - 1], list[index]);
                    list.RemoveAt(list.Count - 1);
                    index--;
                }
                else
                    index--;
            }
        }

        public int NearestIndexAtTime(Timecode time)
        {

            int n = list.Count;

            // Corner cases
            if (time <= list[0].Time)
                return 0;
            if (time >= list[n - 1].Time)
                return n - 1;

            // Doing binary search
            int i = 0, j = n, mid = 0;
            while (i < j)
            {
                mid = (i + j) / 2;

                if (list[mid].Time == time)
                    return mid;

                // If time is less than list element, then search in left
                if (time < list[mid].Time)
                {

                    // If time is greater than previous to mid, return closest of two
                    if (mid > 0 && time > list[mid - 1].Time)
                        return GetClosest(mid - 1,
                                     mid, time);

                    // Repeat for left half
                    j = mid;
                }
                else
                {
                    if (mid < n - 1 && time < list[mid + 1].Time)
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
            return list.IndexOf(keyframe);
        }
        public void RemoveNearestAtTime(Timecode time)
        {
            list.RemoveAt(NearestIndexAtTime(time));
        }
        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }
        public void RemoveRange(int index, int count)
        {
            list.RemoveRange(index, count);
        }
        public Keyframe<T> NearestAtTime(Timecode time)
        {
            return list[NearestIndexAtTime(time)];
        }
        public Keyframe<T> At(int index)
        {
            return list.ElementAt(0);
        }
        public Keyframe<T> this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public int Count => list.Count;
        public bool IsReadOnly => false;
        public void Add(Keyframe<T> keyframe)
        {
            list.Add(keyframe);
            SortList();
            keyframe.TimeChanged += new EventHandler<EventArgs>(Keyframe_TimeChanged);
        }
        public void Clear()
        {
            list.Clear();
        }
        public bool Contains(Keyframe<T> keyframe)
        {
            return list.Contains(keyframe);
        }
        public void CopyTo(Keyframe<T>[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }
        public bool Remove(Keyframe<T> keyframe)
        {
            return list.Remove(keyframe);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
        public IEnumerator<Keyframe<T>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        private void Keyframe_TimeChanged(object sender, EventArgs e)
        {
            SortList();
        }
    }
}
