using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;
using static FelicidApp.Utils.Extensions.FunctionalExtensions;

namespace FelicidApp.Utils.Extensions
{
    public static class CommandExtensions
    {
        public static RelayCommand SetCommand(
            ref RelayCommand command,
            Action beforeAction, Func<Task> function, Action finalAction, string error)
                => command = command ??
                    new RelayCommand(
                        async () => await WardAsync(beforeAction, function, finalAction, error));

        /// <summary>
        /// Creates a new command if none exists. The command will execute a function while dealing with any 
        /// potential errors while running it. 
        /// </summary>
        /// <param name="command">The previous command, if any</param>
        /// <param name="canExecute">The function that determines if the command can be executed</param>
        /// <param name="beforeAction">The action to execute before executing the main function of the command</param>
        /// <param name="function">The main function that the command executes</param>
        /// <param name="finalAction">The final action to execute whether or not the main function succeeded</param>
        /// <param name="error">The error message to show if an exception while running the main function raises</param>
        /// <returns>The previous command, or a new one if none exists</returns>
        public static RelayCommand SetCommand(
            ref RelayCommand command,
            Func<bool> canExecute, 
            Action beforeAction, Func<Task> function, Action finalAction, string error)
                => command = command ?? 
                    new RelayCommand(
                        async () => await WardAsync(beforeAction, function, finalAction, error), 
                        canExecute);

        public static RelayCommand<T> SetCommand<T>(
            ref RelayCommand<T> command,
            Action beforeAction, Func<T, Task> function, Action finalAction, string error)
                => command = command ??
                    new RelayCommand<T>(
                        async (param) => await WardAsync(beforeAction, function, param, finalAction, error));

        public static RelayCommand<T> SetCommand<T>(
            ref RelayCommand<T> command,
            Func<T, bool> canExecute,
            Action beforeAction, Func<T, Task> function, Action finalAction, string error)
                => command = command ??
                    new RelayCommand<T>(
                        async (param) => await WardAsync(beforeAction, function, param, finalAction, error),
                        canExecute);

        public static void RaiseCanExecuteChanged(params dynamic[] commands)
            => commands.ForEach(command => command.RaiseCanExecuteChanged());
    }
}
