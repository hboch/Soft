using Moq;
using Prism.Events;
using Soft.Ui.Bc.BcCustomer;
using Soft.Ui.Bc.Shared;
using Soft.Ui.Shared.Events;
using Soft.Ui.Tests.Extensions;
using Soft.Ui.ViewModel;
using Soft.Ui.ViewModel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Soft.Ui.Tests.ViewModel
{
    public class MainViewModelTests
    {
        private MainViewModel _mainViewModel;
        private Mock<IMainNavigationViewModel> _mainNavigationViewModelMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;

        private List<Mock<ICustomerDetailViewModel>> _customerDetailViewModelMocks;
        private Mock<IViewModelFactory> _viewModelFactoryMock;

        private EventOpenNavigationOrDetailViewModel _eventOpenNavigationOrDetailViewModel;
        private EventAfterDetailDeleted _eventAfterDetailDeleted;
        private EventAfterDetailClose _eventAfterDetailClose;

        public MainViewModelTests()
        {
            _mainNavigationViewModelMock = new Mock<IMainNavigationViewModel>();

            _customerDetailViewModelMocks = new List<Mock<ICustomerDetailViewModel>>();
            _viewModelFactoryMock = new Mock<IViewModelFactory>();
            _viewModelFactoryMock.Setup(vmf => vmf.Create(It.IsAny<string>())).Returns(CreateCustomerDetailViewModelMock());

            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventOpenNavigationOrDetailViewModel = new EventOpenNavigationOrDetailViewModel();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventOpenNavigationOrDetailViewModel>()).Returns(_eventOpenNavigationOrDetailViewModel);
            _eventAfterDetailDeleted = new EventAfterDetailDeleted();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailDeleted>()).Returns(_eventAfterDetailDeleted);
            _eventAfterDetailClose = new EventAfterDetailClose();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailClose>()).Returns(_eventAfterDetailClose);

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            _mainViewModel = new MainViewModel
                (
                _mainNavigationViewModelMock.Object,
                _eventAggregatorMock.Object,
                _messageDialogServiceMock.Object,
                _viewModelFactoryMock.Object
                );
        }

        private IViewModel CreateCustomerDetailViewModelMock()
        {
            var customerDetailViewModelMock = new Mock<ICustomerDetailViewModel>();

            //Mock function DetailViewModel.LoadAsync(CustomerId) call with CustomerId=100 to throw an exception:
            customerDetailViewModelMock.Setup(vm => vm.LoadAsync(It.Is<int>(intId => intId == 100))).Returns(Task.FromResult(false));

            //Mock function DetailViewModel.LoadAsync(CustomerId) call with CustomerId between 0 (new customer) 
            //and 99 to mockup DetailViewModel props Id an ViewModelName:            
            customerDetailViewModelMock.Setup(vm => vm.LoadAsync(It.Is<int>(intId => intId >= 0 && intId < 100))).Returns(Task.FromResult(true))
            .Callback<int?>(customerId =>
            {
                customerDetailViewModelMock.Setup(vmm => vmm.Id).Returns(customerId.HasValue ? customerId.Value : 0);

                customerDetailViewModelMock.Setup(vmm => vmm.Name).Returns(nameof(CustomerDetailViewModel));
            });
            _customerDetailViewModelMocks.Add(customerDetailViewModelMock);
            return customerDetailViewModelMock.Object;
        }

        [Fact]
        public void Load_When_Called_Should_Call_MainNavigationViewModel_Load()
        {
            //Act
            _mainViewModel.Load();

            //Assert
            _mainNavigationViewModelMock.Verify(nvmm => nvmm.Load(), Times.Once);
        }

        [Fact]
        public void EventOpenNavigationOrDetailViewModel_When_Raised_For_CustomerDetailViewModel_Should_Raise_PropertyChanged_For_SelectedDetailViewModel()
        {
            // Arrange
            var anyMockedUpCusomerId = 2;

            //Act
            Action publishEvent = () => _eventOpenNavigationOrDetailViewModel.Publish(
                   new EventOpenNavigationOrDetailViewModelArgs
                   {
                       Id = anyMockedUpCusomerId,
                       ViewModelName = nameof(CustomerDetailViewModel)
                   });
            var fired = _mainViewModel.IsPropertyChangedFired(() => publishEvent(), nameof(_mainViewModel.SelectedViewModel));

            //Assert
            Assert.True(fired);
        }

        [Fact]
        public void EventOpenNavigationOrDetailViewModel_When_Raised_For_CustomerDetailViewModel_Should_Set_IsMainNavigationViewShown_To_False()
        { 
            // Arrange
            var anyMockedUpCusomerId = 2;

            //Act
            _mainViewModel.IsMainNavigationViewShown = true;
            _eventOpenNavigationOrDetailViewModel.Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = anyMockedUpCusomerId, ViewModelName = nameof(CustomerDetailViewModel) });

            //Assert
            Assert.False(_mainViewModel.IsMainNavigationViewShown);            
        }

        [Fact]
        public void EventOpenNavigationOrDetailViewModel_When_Raised_For_CustomerNavigationViewModel_Should_Raise_PropertyChanged_For_SelectedDetailViewModel()
        {
            // Arrange
            const int IdIsAlways0ForNavigationViewModel = 0;

            //Act
            Action publishEvent = () => _eventOpenNavigationOrDetailViewModel.Publish(
                   new EventOpenNavigationOrDetailViewModelArgs
                   {
                       Id = IdIsAlways0ForNavigationViewModel,
                       ViewModelName = nameof(CustomerNavigationViewModel)
                   });
            var fired = _mainViewModel.IsPropertyChangedFired(() => publishEvent(), nameof(_mainViewModel.SelectedViewModel));

            //Assert
            Assert.True(fired);
        }

        [Fact]
        public void EventOpenNavigationOrDetailViewModel_When_Raised_For_CustomerDetailViewModel_Should_Create_CustomerDetailViewModel_And_Call_LoadAsync()
        {
            // Arrange
            var anyMockedUpCusomerId = 2;

            //Act
            _eventOpenNavigationOrDetailViewModel.Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = anyMockedUpCusomerId, ViewModelName = nameof(CustomerDetailViewModel) });

            //Assert
            Assert.NotNull(_mainViewModel.SelectedViewModel);
            Assert.Single(_customerDetailViewModelMocks);
            _customerDetailViewModelMocks.First().Verify(nvm => nvm.LoadAsync(anyMockedUpCusomerId), Times.Once);
        }

        [Fact]
        public void EventOpenNavigationOrDetailViewModel_When_Raised_For_CustomerDetailViewModel_With_NoneExisting_CustomerId_Should_Display_Msg_And_Call_NavigationViewModel_LoadAsync()
        {
            // Arrange
            var Id100ThrowsExceptionInDetailViewModelMockLoadAsyncCall = 100;

            //Act
            _eventOpenNavigationOrDetailViewModel.Publish(new EventOpenNavigationOrDetailViewModelArgs
            {
                Id = Id100ThrowsExceptionInDetailViewModelMockLoadAsyncCall,
                ViewModelName = nameof(CustomerDetailViewModel)
            });

            //Assert
            _messageDialogServiceMock.Verify(ms => ms.ShowInfoDialog("Entry could not be loaded as it might have been deleted. Displayed Entries are refreshed.", "Information"));
        }

        [Fact]
        public void EventOpenNavigationOrDetailViewModel_When_Raised_For_CustomerDetailViewModel_Should_Add_CustomerDetailViewModel_To_ViewModels()
        {
            // Arrange
            var anyMockedUpCusomerId = 2;

            //Act
            _eventOpenNavigationOrDetailViewModel.Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = anyMockedUpCusomerId, ViewModelName = nameof(CustomerDetailViewModel) });

            //Assert
            Assert.NotNull(_mainViewModel.SelectedViewModel);
            Assert.Single(_mainViewModel.ViewModels);
        }

        [Fact]
        public void EventOpenNavigationOrDetailViewModel_When_Raised_For_2_Customers_Should_Add_2_DetailViewModels_To_DetailViewModels()
        {
            // Arrange
            var customerId1 = 1;
            var customerId2 = 2;

            //Act
            _eventOpenNavigationOrDetailViewModel.Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = customerId1, ViewModelName = nameof(CustomerDetailViewModel) });
            _eventOpenNavigationOrDetailViewModel.Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = customerId2, ViewModelName = nameof(CustomerDetailViewModel) });

            //Assert
            Assert.NotNull(_mainViewModel.SelectedViewModel);
            Assert.Equal(2, _mainViewModel.ViewModels.Count);
        }

        [Fact]
        public void EventOpenNavigationOrDetailViewModel_When_Raised_Twice_For_Same_CustomerDetailViewModel_Should_Add_To_DetailViewModels_Only_Once()
        {
            // Arrange
            var customernId1 = 1;

            //Act
            _eventOpenNavigationOrDetailViewModel.Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = customernId1, ViewModelName = nameof(CustomerDetailViewModel) });
            _eventOpenNavigationOrDetailViewModel.Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = customernId1, ViewModelName = nameof(CustomerDetailViewModel) });

            //Assert
            Assert.NotNull(_mainViewModel.SelectedViewModel);
            Assert.Single(_mainViewModel.ViewModels);
            Assert.Single(_customerDetailViewModelMocks);
            _customerDetailViewModelMocks.First().Verify(k => k.LoadAsync(customernId1), Times.Once);
        }

        [Fact]
        public void EventAfterDetailDeleted_When_Raised_Should_Remove_CustomerDetailViewModel_From_ViewModels()
        {
            // Arrange
            var anyMockedUpCusomerId = 2;
            _eventOpenNavigationOrDetailViewModel.Publish(new EventOpenNavigationOrDetailViewModelArgs { Id = anyMockedUpCusomerId, ViewModelName = nameof(CustomerDetailViewModel) });
            Assert.Contains(_mainViewModel.ViewModels, vm => vm.Id == anyMockedUpCusomerId);

            //Act
            _eventAfterDetailDeleted.Publish(
                new EventAfterDetailDeletedArgs
                {
                    Id = anyMockedUpCusomerId,
                    ViewModelName = nameof(CustomerDetailViewModel)
                });

            //Assert
            Assert.DoesNotContain(_mainViewModel.ViewModels, vm => vm.Id == anyMockedUpCusomerId);
        }

        [Fact]
        public void EventAfterDetailClose_When_Raised_Should_Remove_CustomerDetailViewModel_From_ViewModels()
        {
            // Arrange
            var anyMockedUpCusomerId = 2;
            _eventOpenNavigationOrDetailViewModel.Publish(
                new EventOpenNavigationOrDetailViewModelArgs { Id = anyMockedUpCusomerId, ViewModelName = nameof(CustomerDetailViewModel) });
            Assert.Contains(_mainViewModel.ViewModels, vm => vm.Id == anyMockedUpCusomerId);

            //Act
            _eventAfterDetailClose.Publish(
                new EventAfterDetailCloseArgs
                {
                    Id = anyMockedUpCusomerId,
                    ViewModelName = nameof(CustomerDetailViewModel)
                });

            //Assert
            Assert.DoesNotContain(_mainViewModel.ViewModels, vm => vm.Id == anyMockedUpCusomerId);
        }

        [Fact]
        public void Ctor_Should_Set_Property_ViewModels_Not_Null()
        {
            Assert.NotNull(_mainViewModel.ViewModels);
        }

        [Fact]
        public void Ctor_Should_Set_CommandCreateSingleDetailView()
        {
            Assert.NotNull(_mainViewModel.CommandCreateSingleDetailView);
        }

        [Fact]
        public void Ctor_Should_Set_IsMainNavigationViewShown_To_False()
        {
            Assert.False(_mainViewModel.IsMainNavigationViewShown);
        }

        [Fact]
        public void CommandCreateSingleDetailView_When_Executed_With_ViewModel_Type_Should_Publish_EventOpenNavigationOrDetailViewModel_Id_0_And_ViewModel()
        {
            //Arrange
            MainViewModel mainViewModel;

            Mock<EventOpenNavigationOrDetailViewModel> eventOpenNavigationOrDetailViewModelMock;
            eventOpenNavigationOrDetailViewModelMock = new Mock<EventOpenNavigationOrDetailViewModel>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventOpenNavigationOrDetailViewModel>()).Returns(eventOpenNavigationOrDetailViewModelMock.Object);

            mainViewModel = new MainViewModel
                (
                _mainNavigationViewModelMock.Object,
                _eventAggregatorMock.Object,
                _messageDialogServiceMock.Object,
                _viewModelFactoryMock.Object
                );

            //Act
            //Test should suceed with any ViewModel for Act and in Assert Verify function AboutViewModel is used here:
            _mainViewModel.CommandCreateSingleDetailView.Execute(typeof(AboutViewModel));

            //Assert
            eventOpenNavigationOrDetailViewModelMock.Verify(
                eopnodv => eopnodv.Publish(It.Is<EventOpenNavigationOrDetailViewModelArgs>
                (eopnodva => eopnodva.Id == 0 && eopnodva.ViewModelName == nameof(AboutViewModel))), Times.Once);
        }

        [Fact]
        public void CommandCreateSingleDetailView_When_Executed_With_None_ViewModel_Type_Should_Not_Publish_EventOpenNavigationOrDetailViewModel()
        {
            //Arrange
            MainViewModel mainViewModel;

            Mock<EventOpenNavigationOrDetailViewModel> eventOpenNavigationOrDetailViewModelMock;
            eventOpenNavigationOrDetailViewModelMock = new Mock<EventOpenNavigationOrDetailViewModel>();
            _eventAggregatorMock.Setup(
                eam => eam.GetEvent<EventOpenNavigationOrDetailViewModel>()).Returns(eventOpenNavigationOrDetailViewModelMock.Object);

            mainViewModel = new MainViewModel
                (
                _mainNavigationViewModelMock.Object,
                _eventAggregatorMock.Object,
                _messageDialogServiceMock.Object,
                _viewModelFactoryMock.Object
                );

            //Act
            //Test should suceed with any ViewModel for Act and in Assert Verify function AboutViewModel is used here:
            _mainViewModel.CommandCreateSingleDetailView.Execute(typeof(int));

            //Assert
            eventOpenNavigationOrDetailViewModelMock.Verify(
                eonodvmm => eonodvmm.Publish(It.Is<EventOpenNavigationOrDetailViewModelArgs>
                (eonodvma => eonodvma.Id == It.IsAny<int>() && eonodvma.ViewModelName == It.IsAny<string>())), Times.Never);
        }
    }
}
