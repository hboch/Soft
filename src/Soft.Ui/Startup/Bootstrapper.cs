using Autofac;
using Prism.Events;
using Soft.DataAccess;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using Soft.Ui.Bc.BcAccountManager;
using Soft.Ui.Bc.BcBroker;
using Soft.Ui.Bc.BcCustomer;
using Soft.Ui.Bc.Shared;
using Soft.Ui.ViewModel;
using Soft.Ui.ViewModel.Services;

namespace Soft.Ui.Startup
{
    public class Bootstrapper
    {
        /// <summary>
        /// Register classes in Autofac
        /// </summary>
        /// <returns></returns>
        public IContainer Bootstrap()
        {
            //TODO Wie testen ?
            var builder = new ContainerBuilder();

            //register class for interface using single instance scope, one instance is returned from all requests in the root and all nested scopes.
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            //register classes as the type itself
            builder.RegisterType<SoftDbContext>().AsSelf();

            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<MainWindow>().AsSelf();

            //register a set of classes for the same interface (they implement)
            builder.RegisterType<CustomerRepository>().AsImplementedInterfaces();
            builder.RegisterType<BrokerRepository>().AsImplementedInterfaces();
            builder.RegisterType<AccountManagerRepository>().AsImplementedInterfaces();

            //register class for specific interface 
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            builder.RegisterType<MainNavigationViewModel>().As<IMainNavigationViewModel>();
            builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>();

            //register class for specific interface<of T>
            builder.RegisterType<LookupDataServiceCustomer>().As<IGenericLookupDataService<Customer>>();
            builder.RegisterType<LookupDataServiceAccountManager>().As<IGenericLookupDataService<AccountManager>>();
            builder.RegisterType<LookupDataServiceBroker>().As<IGenericLookupDataService<Broker>>();

            //register class for interface identified by an object key 
            //in this application required for every Navigation- and DetailViewModel
            builder.RegisterType<AboutViewModel>().Keyed<IViewModel>(nameof(AboutViewModel));
            builder.RegisterType<CustomerNavigationViewModel>().Keyed<IViewModel>(nameof(CustomerNavigationViewModel));
            builder.RegisterType<CustomerDetailViewModel>().Keyed<IViewModel>(nameof(CustomerDetailViewModel));
            builder.RegisterType<BrokerNavigationViewModel>().Keyed<IViewModel>(nameof(BrokerNavigationViewModel));
            builder.RegisterType<BrokerDetailViewModel>().Keyed<IViewModel>(nameof(BrokerDetailViewModel));
            builder.RegisterType<AccountManagerNavigationViewModel>().Keyed<IViewModel>(nameof(AccountManagerNavigationViewModel));
            builder.RegisterType<AccountManagerDetailViewModel>().Keyed<IViewModel>(nameof(AccountManagerDetailViewModel));
            
            return builder.Build();
        }
    }
}
