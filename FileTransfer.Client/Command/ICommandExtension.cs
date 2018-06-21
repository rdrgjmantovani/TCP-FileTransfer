using System.Windows.Input;

namespace FileTransfer.Client.Command
{
    public static class ICommandExtension
    {
        public static void RaiseRequerySuggested(this ICommand command) =>
            CommandManager.InvalidateRequerySuggested();
    }
}
