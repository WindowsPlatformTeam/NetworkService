using NetworkService.Contracts;
using NetworkService.Contracts.Models.Exceptions.Specifics;
using NetworkService.TestRunner.Models;
using System.Collections.ObjectModel;

namespace NetworkService.TestRunner.ViewModel
{
    public class NetworkServiceViewModel : TestRunnerViewModelBase
    {
        #region Fields

        #endregion

        #region Constructor
        public NetworkServiceViewModel(string name, INetworkService networkService) : base(name, networkService)
        {
            InitializeMethods();
        }
        #endregion

        #region Private Methods
        private void InitializeMethods()
        {
            ServiceMethods = new ObservableCollection<Service>
            {
                new Service
                {
                    Name = "Get",
                    ServiceMethod = async () =>
                    {
                        var url = "http://localhost:56421/api/test/get-test-boolean";
                        try
                        {
                            return await _networkService.Get<bool, ErrorModel>(url);
                        }
                        catch(System.Exception e)
                        {
                            return e;
                        }
                    }
                },
                new Service
                {
                    Name = "Post",
                    ServiceMethod = async () =>
                    {
                        var url = "http://localhost:56421/api/test/post-test-boolean";
                        try
                        {
                            return await _networkService.Post<User, User, ErrorModel>(url, new User());
                        }
                        catch(System.Exception e)
                        {
                            return e;
                        }
                    }
                },
                new Service
                {
                    Name = "NotFound",
                    ServiceMethod = async () =>
                    {
                        var url = "http://localhost:56421/api/test/get-test-booleannnnnn";
                        try {
                            return await _networkService.Get<bool, ErrorModel>(url);

                        }
                        catch(NetworkServiceNotFoundException e)
                        {
                            return e.Message;
                        }
                    }
                },
            };
        }
    }
    #endregion
}
