using Engine.Attributes;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace Engine.Core
{
    public interface ICommand
    {
        string Name { get; }
        void Execute();
        void Undo();
    }

    public static class CommandManager
    {
        // TODO: limit size with deque
        private static Stack<CommandGroup> _undoGroups = new();
        private static Stack<CommandGroup> _redoGroups = new();
        private static bool _groupOpened;
        public static Stack<bool> IgnoreStack { get; set; } = new();

        public static IEnumerable<CommandGroup> UndoGroups
        {
            get
            {
                foreach (CommandGroup group in _undoGroups)
                    yield return group;
            }
        }

        public static IEnumerable<CommandGroup> RedoGroups
        {
            get
            {
                foreach (CommandGroup group in _redoGroups)
                    yield return group;
            }
        }

        public static void Undo()
        {
            if (_undoGroups.Count < 1)
                return;

            var undoGroup = _undoGroups.Pop();
            _redoGroups.Push(undoGroup);
            foreach (var command in undoGroup.ReverseCommands)
                command.Undo();
        }

        public static void Redo()
        {
            if (_redoGroups.Count < 1)
                return;

            var redoGroup = _redoGroups.Pop();
            _undoGroups.Push(redoGroup);
            foreach (var command in redoGroup.Commands)
                command.Execute();
        }

        public static void Execute(ICommand command)
        {
            command.Execute();
            _redoGroups.Clear();

            if (IgnoreStack.Count > 0 && IgnoreStack.Peek())
                return;

            if (_groupOpened)
                _undoGroups.Peek().Commands.AddLast(command);
            else
            {
                /*bool sameType = true;
                if (_undoGroups.Count > 0)
                {
                    var lastGroup = _undoGroups.Peek();

                    foreach (var lastCommand in lastGroup.Commands)
                    {
                        if (lastCommand.GetType() != command.GetType())
                        {
                            sameType = false;
                            break;
                        }
                    }

                    if (sameType)
                    {
                        lastGroup.Commands.AddLast(command);
                        return;
                    }
                }*/

                var group = new CommandGroup(command.Name);
                group.Commands.AddLast(command);
                _undoGroups.Push(group);
            }
        }

        public static void ExecuteSetter<T>(string name, T oldValue, T newValue, Action<T> setter)
        {
            ExecuteIfNeeded(oldValue, newValue, new ValueChangeCommand<T>(name, oldValue, newValue, setter));
        }

        public static void ExecuteIfNeeded<T>(T oldValue, T newValue, ICommand command)
        {
            // TODO: can crash
            if (oldValue.Equals(newValue))
                return;

            Execute(command);
        }

        public static void BeginGroup(string name)
        {
            // TODO: use name
            _undoGroups.Push(new CommandGroup(name));
            _groupOpened = true;
        }

        public static void EndGroup()
        {
            _groupOpened = false;
        }

    }

    public class ValueChangeCommand<T> : ICommand
    {
        private string _name;
        private T _oldValue;
        private T _newValue;
        private Action<T> _setValue;

        public string Name => _name;

        public void Execute()
        {
            _setValue(_newValue);
        }

        public void Undo()
        {
            _setValue(_oldValue);
        }

        public ValueChangeCommand(string name, T oldValue, T newValue, Action<T> setValue)
        {
            _name = name;
            _oldValue = oldValue;
            _newValue = newValue;
            _setValue = setValue;
        }
    }

    public struct CommandGroup
    {
        public string Name { get; init; }
        public LinkedList<ICommand> Commands { get; init; } = new();
        public IEnumerable<ICommand> ReverseCommands
        {
            get
            {
                var el = Commands.Last;
                while (el != null)
                {
                    yield return el.Value;
                    el = el.Previous;
                }
            }
        }
        public Type? Type { get; init; } = null;

        public CommandGroup(string name, LinkedList<ICommand> commands)
        {
            Name = name;
            Commands = commands;
        }

        public CommandGroup(string name)
        {
            Name = name;
        }
    }
}
