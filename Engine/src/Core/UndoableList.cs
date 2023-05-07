using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class UndoableList<T> : IList<T>
    {
        private readonly List<T> _list = new();

        public T this[int index] { get => _list[index]; set => _list[index] = value; }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(T item) => CommandManager.Execute(new InsertCommand(_list, _list.Count, item));

        public void Clear() => CommandManager.Execute(new ClearCommand(_list));

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item) => CommandManager.Execute(new InsertCommand(_list, index, item));

        public bool Remove(T item)
        {
            int index = _list.IndexOf(item);
            if (index != -1)
            {
                CommandManager.Execute(new RemoveAtCommand(_list, index));
                return true;
            }

            return false;
        }

        public void RemoveAt(int index) => CommandManager.Execute(new RemoveAtCommand(_list, index));

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        public class InsertCommand : ICommand
        {
            private List<T> _list;
            private int _index;
            private T _item;

            public string Name => "Added " + _item?.GetType().Name ?? typeof(T).Name;

            public void Execute()
            {
                _list.Insert(_index, _item);
            }

            public void Undo()
            {
                _list.RemoveAt(_index);
            }

            public InsertCommand(List<T> list, int index, T item)
            {
                _list = list;
                _index = index;
                _item = item;
            }
        }
        public class RemoveAtCommand : ICommand
        {
            private List<T> _list;
            private int _index;
            private T _removedItem;

            public string Name => $"Removed " + _removedItem?.GetType().Name ?? typeof(T).Name;

            public void Execute()
            {
                _removedItem = _list[_index];
                _list.RemoveAt(_index);
            }

            public void Undo()
            {
                _list.Insert(_index, _removedItem);
            }

            public RemoveAtCommand(List<T> list, int index)
            {
                _list = list;
                _index = index;
            }
        }

        public class ClearCommand : ICommand
        {
            private T[] _oldArray;
            private List<T> _list;

            public string Name => $"Cleared list of type ${typeof(T).Name}";

            public void Execute()
            {
                _list.CopyTo(_oldArray);
                _list.Clear();
            }

            public void Undo()
            {
                foreach (T item in _oldArray)
                {
                    _list.Add(item);
                }
            }

            public ClearCommand(List<T> list)
            {
                _list = list;
            }
        }
    }
}
