using Engine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
            Console.WriteLine("Undo");
            if (_undoGroups.Count < 1)
                return;

            var undoGroup = _undoGroups.Pop();
            _redoGroups.Push(undoGroup);
            foreach(var command in undoGroup.ReverseCommands)
                command.Undo();
        }

        public static void Redo()
        {
            Console.WriteLine("Redo");
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

            if (_groupOpened)
                _undoGroups.Peek().Commands.AddLast(command);
            else
            {
                var group = new CommandGroup(command.Name);
                group.Commands.AddLast(command);
                _undoGroups.Push(group);
            }
        }

        public static void ExecuteIfNeeded<T>(T oldValue, T newValue, ICommand command)
        {
            // TODO: can crash
            if (oldValue!.Equals(newValue))
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
