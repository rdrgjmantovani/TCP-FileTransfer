using System;
using System.Windows.Input;

namespace FileTransfer.Client.Command
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> action, Predicate<object> canExecute)
        {
            _execute = action ?? throw new NullReferenceException($"{nameof(action)} can't be null");
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public bool CanExecute(object parameter) =>
            _canExecute == null ? true : _canExecute.Invoke(parameter);

        public void Execute(object parameter) =>
            _execute.Invoke(parameter);
    }
}
