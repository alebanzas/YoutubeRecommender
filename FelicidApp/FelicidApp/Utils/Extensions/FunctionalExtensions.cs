using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using FelicidApp.Utils.Messages;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

namespace FelicidApp.Utils.Extensions
{
    public static class FunctionalExtensions
    {
        /// <summary>
        /// Executes a function by protecting the app from potential errors. 
        /// If there is an error it sends the error message to whoever is interested in them within the app.
        /// </summary>
        /// <param name="beforeAction">The action to execute before executing the main function</param>
        /// <param name="function">The main function to execute</param>
        /// <param name="finalAction">The final action to execute whether or not the main function succeeded</param>
        /// <param name="error">The error message to show if an exception while running the main function raises</param>
        /// <returns>A task to wait for the function to execute</returns>
        public static async Task WardAsync(
            Action beforeAction, Func<Task> function, Action finalAction, string error)
        {
            beforeAction();
            try
            {
                await function();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new ErrorMessage(error, ex.Message, ex));
            }
            finally
            {
                finalAction();
            }
        }

        public static async Task WardAsync(Func<Task> function, string error)
        {
            try
            {
                await function();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new ErrorMessage(error, ex.Message, ex));
            }
        }

        public static async Task WardAsync<T>(
            Action beforeAction, Func<T, Task> function, T param, Action finalAction, string error)
        {
            beforeAction();
            try
            {
                await function(param);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new ErrorMessage(error, ex.Message, ex));
            }
            finally
            {
                finalAction();
            }
        }

        /// <summary>
        /// Executes code in the UI thread
        /// </summary>
        /// <param name="handler">The code to execute</param>
        public static async void DispatchAsync(DispatchedHandler handler)
        {
            // Better to use CoreApplication.MainView than Window.Current.Dispatcher
            if (CoreApplication.MainView.Dispatcher.HasThreadAccess)
            {
                handler();
            }
            else
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);
            }
        }
    }
}
