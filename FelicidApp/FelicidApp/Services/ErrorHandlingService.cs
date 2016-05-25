using GalaSoft.MvvmLight.Messaging;
using FelicidApp.View.Base;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FelicidApp.Utils.Messages;

namespace FelicidApp.Services
{
    public class ErrorHandlingService
    {
        private static ErrorHandlingService defaultService = new ErrorHandlingService();
        private static DispatcherTimer timer = new DispatcherTimer();

        private static BasePage pageToShowErrors 
            => (Window.Current.Content as Frame).Content as BasePage;

        public static void Initialize()
        {
            timer.Tick += OnTimerTick;
            Messenger.Default.Register<ErrorMessage>(defaultService, defaultService.OnErrorMessage);
        }

        private void OnErrorMessage(ErrorMessage error)
        {
            Debug.WriteLine($"ErrorHandlingService.OnErrorMessage: {error.Title} - {error.Message}");

            pageToShowErrors.ShowErrorToUser(error);
            timer.Interval = TimeSpan.FromMilliseconds(6000);
            timer.Start();
        }

        private static void OnTimerTick(object sender, object e)
        {
            pageToShowErrors.ShowErrorToUser(null);
            timer.Stop();
        }
    }
}
