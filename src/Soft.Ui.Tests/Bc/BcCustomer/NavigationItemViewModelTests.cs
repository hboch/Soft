using Moq;
using Prism.Events;
using Soft.Ui.Shared.Events;
using Soft.Ui.Tests.Extensions;
using Soft.Ui.ViewModel;
using Xunit;

namespace Soft.Ui.Tests.Bc.BcCustomer
{
    public class NavigationItemViewModelTests
    {
        private Mock<EventOpenNavigationOrDetailViewModel> _eventOpenNavigationOrDetailViewModel;
        private NavigationItemViewModel _viewModel;

        const int anyLookupItemId = 2;
        const string anyLookupItemDisplayMember = "AnyString";
        const string anyViewModelName = "AnyViewModelName";

        public NavigationItemViewModelTests()
        {
            var eventAggregatorMock = new Mock<IEventAggregator>();

            _eventOpenNavigationOrDetailViewModel = new Mock<EventOpenNavigationOrDetailViewModel>();
            eventAggregatorMock.Setup(eam => eam.GetEvent<EventOpenNavigationOrDetailViewModel>()).Returns(_eventOpenNavigationOrDetailViewModel.Object);

            _viewModel = new NavigationItemViewModel(anyLookupItemId, anyLookupItemDisplayMember, anyViewModelName, eventAggregatorMock.Object);
        }

        [Fact]
        public void DisplayMember_When_Set_Should_Raise_PropertyChangedEvent_For_DisplayMember()
        {
            //Act
            var fired = _viewModel.IsPropertyChangedFired(() => { _viewModel.DisplayMember = "AnyOtherString"; }, nameof(_viewModel.DisplayMember));

            //Assert
            Assert.True(fired);
        }

        [Fact]
        public void CommandOpenDetailViewModel_When_Executed_Should_Publish_EventOpenDetailView_For_IrgendeineLookupItemId_And_IrgendeinViewModelName()
        {
            //Act
            _viewModel.CommandOpenDetailViewModel.Execute(null);

            //Assert
            _eventOpenNavigationOrDetailViewModel.Verify(eonodvm => eonodvm.Publish(It.Is<EventOpenNavigationOrDetailViewModelArgs>(
                eonodvma => eonodvma.Id == anyLookupItemId && eonodvma.ViewModelName == anyViewModelName)), Times.Once);
        }
    }
}
