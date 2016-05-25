using GalaSoft.MvvmLight;

namespace FelicidApp.ViewModel.Base
{
    public class BaseViewModel : ObservableObject
    {
        public BaseViewModel() { }

        public virtual void OnPageLoaded() { }

        public virtual void OnNavigatedTo(object navigationContext) { }

        public virtual void OnNavigatedFrom() { }
    }
}
