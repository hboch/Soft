using Moq;
using Prism.Events;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using Soft.Model.Shared;
using Soft.Ui.Bc.BcCustomer;
using Soft.Ui.Shared.Events;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Soft.Ui.Tests.Bc.BcCustomer
{
    public class CustomerNavigationViewModelTests
    {
        private CustomerNavigationViewModel _customerNavigationViewModel;
        private Mock<IGenericLookupDataService<Customer>> _customerLookupDataServiceMock;
        
        private EventAfterDetailSaved _eventAfterSavedCustomer;
        private EventAfterDetailDeleted _eventAfterDeletedCustomer;
        private Mock<EventAfterDetailClose> _eventAfterDetailCloseMock;
        private Mock<EventOpenNavigationOrDetailViewModel> _eventOpenNavigationOrDetailViewModelMock;

        private List<LookupItem> _lookupItemList;

        public CustomerNavigationViewModelTests()
        {
            var eventAggregatorMock = new Mock<IEventAggregator>();

            _eventAfterSavedCustomer = new EventAfterDetailSaved();
            _eventAfterDeletedCustomer = new EventAfterDetailDeleted();
            _eventAfterDetailCloseMock = new Mock<EventAfterDetailClose>();
            _eventOpenNavigationOrDetailViewModelMock = new Mock<EventOpenNavigationOrDetailViewModel>();

            eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailSaved>()).Returns(_eventAfterSavedCustomer);
            eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailDeleted>()).Returns(_eventAfterDeletedCustomer);
            eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailClose>()).Returns(_eventAfterDetailCloseMock.Object);
            eventAggregatorMock.Setup(ea => ea.GetEvent<EventOpenNavigationOrDetailViewModel>()).Returns(_eventOpenNavigationOrDetailViewModelMock.Object);

            _customerLookupDataServiceMock = new Mock<IGenericLookupDataService<Customer>>();

            _lookupItemList = new List<LookupItem>
                {
                    new LookupItem {Id=1, DisplayMember = "Julia" },
                    new LookupItem {Id=2, DisplayMember = "Thomas" }
                };
            _customerLookupDataServiceMock.Setup(dp => dp.GetEntityLookupAsync()).ReturnsAsync(_lookupItemList);

            _customerNavigationViewModel = new CustomerNavigationViewModel(eventAggregatorMock.Object, _customerLookupDataServiceMock.Object);
        }

        [Fact]
        public async void LoadAsync_When_Called_Should_Load_The_MockedUp_CustomerNavigationItemViewModels()
        {
            //Act
            await _customerNavigationViewModel.LoadAsync(id:0);

            //Assert
            Assert.Equal(_lookupItemList.Count(), _customerNavigationViewModel.EntityNavigationItemViewModels.Count());

            //Get _lookupItemList item with Id=1:
            var customerNavigationItemViewModel = _customerNavigationViewModel.EntityNavigationItemViewModels.SingleOrDefault(f => f.Id == 1);
            Assert.NotNull(customerNavigationItemViewModel);
            Assert.Equal("Julia", customerNavigationItemViewModel.DisplayMember);

            //Get _lookupItemList item with Id=2:
            customerNavigationItemViewModel = _customerNavigationViewModel.EntityNavigationItemViewModels.SingleOrDefault(f => f.Id == 2);
            Assert.NotNull(customerNavigationItemViewModel);
            Assert.Equal("Thomas", customerNavigationItemViewModel.DisplayMember);
        }

        [Fact]
        public async void LoadAsync_When_Called_Twice_Should_Load_The_MockedUp_CustomerNavigationItemViewModels_Only_Once()
        {
            //Act
            await _customerNavigationViewModel.LoadAsync(id:0);
            await _customerNavigationViewModel.LoadAsync(id:0);

            //Assert
            Assert.Equal(2, _customerNavigationViewModel.EntityNavigationItemViewModels.Count);
        }

        [Fact]
        public async void EventAfterSavedCustomer_When_Published_Should_Call_ReLoadAsync()
        {
            //Arrange
            //Load customerNavigationItemViewModels to use the first item for this test:
            await _customerNavigationViewModel.LoadAsync(id:0);
            var customerNavigationItemViewModel = _customerNavigationViewModel.EntityNavigationItemViewModels.First();
            var customerNavigationItemViewModelItemId = customerNavigationItemViewModel.Id;
            var expectedDisplayMember = customerNavigationItemViewModel.DisplayMember;

            //Act
            customerNavigationItemViewModel.DisplayMember = "Different Name";
            _eventAfterSavedCustomer.Publish(                
                new EventAfterDetailSavedArgs
                {
                    Id = customerNavigationItemViewModelItemId,
                    ViewModelName = nameof(CustomerDetailViewModel)
                });

            Assert.Equal("Different Name", customerNavigationItemViewModel.DisplayMember);
            customerNavigationItemViewModel = _customerNavigationViewModel.EntityNavigationItemViewModels.First();
            Assert.Equal(expectedDisplayMember, customerNavigationItemViewModel.DisplayMember);
        }

        [Fact]
        public async void EventAfterDeletedCustomer_When_Published_With_CustomerId_Should_Remove_Customer_From_EntityNavigationItemViewModels()
        {
            //Arrange
            //Load customerNavigationItemViewModels to use the first item for this test:
            await _customerNavigationViewModel.LoadAsync(id:0);
            var customerNavigationItemViewModel = _customerNavigationViewModel.EntityNavigationItemViewModels.First();
            var customerNavigationItemViewModelItemId = customerNavigationItemViewModel.Id;

            //Act
            _eventAfterDeletedCustomer.Publish(
                new EventAfterDetailDeletedArgs
                {
                    Id = customerNavigationItemViewModelItemId,
                    ViewModelName = nameof(CustomerDetailViewModel)
                });

            //Assert
            var actual = _customerNavigationViewModel.EntityNavigationItemViewModels.Any(k => k.Id == customerNavigationItemViewModelItemId);
            Assert.False(actual, "ViewModelItem was not deleted");
        }

        [Fact]
        public void CommandClose_When_Called_Should_Publish_EventAfterDetailClose()
        {
            //Arrange
            const int IdIsAlways0ForNavigationViewModel = 0;

            //Act
            _customerNavigationViewModel.CommandClose.Execute(null);

            //Assert
            _eventAfterDetailCloseMock.Verify(
                     akse => akse.Publish(It.Is<EventAfterDetailCloseArgs>
                     (aksea => aksea.Id == IdIsAlways0ForNavigationViewModel
                            && aksea.ViewModelName == _customerNavigationViewModel.GetType().Name)),
                     Times.Once);
        }

        [Fact]
        public void CommandCreateNewDetail_When_Called_Should_Publish_EventOpenNavigationOrDetailViewModel()
        {
            //Arrange
            const int IdIsAlways0ForNavigationViewModel = 0;

            //Act
            _customerNavigationViewModel.CommandCreateNewDetail.Execute(null);

            //Assert
            _eventOpenNavigationOrDetailViewModelMock.Verify(
                     eonodv => eonodv.Publish(It.Is<EventOpenNavigationOrDetailViewModelArgs>
                     (eonodva => eonodva.Id == IdIsAlways0ForNavigationViewModel
                            && eonodva.ViewModelName == _customerNavigationViewModel.NameOfDetailViewModel())),
                     Times.Once);
        }

    }
}
