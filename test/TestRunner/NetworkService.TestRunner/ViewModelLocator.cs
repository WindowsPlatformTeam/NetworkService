using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using NetworkService.Contracts;
using NetworkService.Contracts.Adapters;
using NetworkService.Impl.Adapters;
using NetworkService.TestRunner.ViewModel;
using System.Net;

namespace NetworkService.TestRunner
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            RegisterBindings();
            RegisterInternalServices();
            RegisterViewModels();
        }

        #region ViewModels

        public NetworkServiceViewModel NetworkServiceViewModel => ServiceLocator.Current.GetInstance<NetworkServiceViewModel>();

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();


        #endregion

        public static void Cleanup() { }

        #region Private Methods
        private void RegisterBindings()
        {
            
        }

        private void RegisterInternalServices()
        {
            SimpleIoc.Default.Register<IHttpClientAdapter>(() => SimpleIoc.Default.GetInstance<HttpClientAdapter>());
            SimpleIoc.Default.Register<INetworkService>(() => SimpleIoc.Default.GetInstance<Impl.NetworkService>());
        }

        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register(() => new NetworkServiceViewModel("Get", new Impl.NetworkService(new HttpClientAdapter())));
        }
        #endregion
    }
}
