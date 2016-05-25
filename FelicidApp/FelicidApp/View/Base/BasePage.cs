using System;
using FelicidApp.Utils.Messages;
using FelicidApp.ViewModel.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FelicidApp.View.Base
{
    public class BasePage : Page
    {
        public BasePage()
        {
            Loaded += OnPageLoaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
            => (DataContext as BaseViewModel)?.OnPageLoaded();

        protected override void OnNavigatedTo(NavigationEventArgs e)
            => (DataContext as BaseViewModel)?.OnNavigatedTo(e.Parameter);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => (DataContext as BaseViewModel)?.OnNavigatedFrom();

        public virtual void ShowErrorToUser(ErrorMessage error) { }
    }
}
