using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NetworkService.Contracts;
using NetworkService.TestRunner.Helper;
using NetworkService.TestRunner.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkService.TestRunner.ViewModel
{
    public class TestRunnerViewModelBase : ViewModelBase
    {
        #region Fields
        private string _serviceResult;
        protected RelayCommand<Service> _executeMethodCommand;
        protected INetworkService _networkService;
        private ObservableCollection<Service> _serviceMethods;
        #endregion

        #region Properties
        public string Name { get; set; }

        public string ServiceResult
        {
            get { return _serviceResult; }
            set
            {
                _serviceResult = value;
                RaisePropertyChanged(() => ServiceResult);
            }
        }

        public ObservableCollection<Service> ServiceMethods
        {
            get { return _serviceMethods; }
            set
            {
                _serviceMethods = value;
                RaisePropertyChanged(() => ServiceMethods);
            }
        }
        #endregion

        #region Commands
        public ICommand ExecuteMethodCommand => _executeMethodCommand;
        #endregion

        #region Constructor
        public TestRunnerViewModelBase(string name, INetworkService networkService)
        {
            _networkService = networkService;
            Name = name;
            InitializeCommands();
        }
        #endregion

        #region Public Methods
        protected async Task<string> MakeRequest(Func<Task<object>> function, params object[] args)
        {
            string serviceResult;
            try
            {
                serviceResult = ClientHelper.SerializeObject(await function());
            }
            catch (Exception e)
            {
                serviceResult = ClientHelper.SerializeObject(e.Message);
            }

            return serviceResult;
        }

        #endregion

        #region Private Methods
        private void InitializeCommands()
        {
            _executeMethodCommand = new RelayCommand<Service>(async itemSelected =>
            {
                ServiceResult = await MakeRequest(async () => await itemSelected.ServiceMethod());
            });
        }
        #endregion
    }
}
