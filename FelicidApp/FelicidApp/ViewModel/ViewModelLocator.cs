using Microsoft.Practices.Unity;

namespace FelicidApp.ViewModel
{
    public class ViewModelLocator
    {
        private UnityContainer container = new UnityContainer();

        public ViewModelLocator()
        {
            container.RegisterType<MainViewModel>();
        }

        public MainViewModel MainViewModel => container.Resolve<MainViewModel>();
    }
}
