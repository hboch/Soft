using Moq;
using Prism.Events;
using Soft.Ui.Shared.Events;
using Soft.Ui.ViewModel;
using Xunit;

namespace Soft.Ui.Tests.ViewModel
{
    public class MainNavigationItemViewModelTests
    {
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<EventOpenNavigationOrDetailViewModel> _eventOpenNavigationOrDetailViewModel;
        private MainNavigationItemViewModel _viewModel;

        const string anyLookupItemDisplayMember = "AnyString";
        const string anyViewModelName = "AnyViewModelName";

        public MainNavigationItemViewModelTests()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();

            _eventOpenNavigationOrDetailViewModel = new Mock<EventOpenNavigationOrDetailViewModel>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventOpenNavigationOrDetailViewModel>()).Returns(_eventOpenNavigationOrDetailViewModel.Object);

            _viewModel = new MainNavigationItemViewModel(anyLookupItemDisplayMember, anyViewModelName, _eventAggregatorMock.Object);
        }

        [Fact]
        public void CommandOpenNavigationViewModel_When_Executed_Should_Publish_EventOpenNavigationView_For_IrgendeinViewModelName()
        {
            //Act
            _viewModel.CommandOpenNavigationViewModel.Execute(anyViewModelName);

            //Assert
            _eventOpenNavigationOrDetailViewModel.Verify(akse => akse.Publish(It.Is<EventOpenNavigationOrDetailViewModelArgs>(eadd
                => eadd.Id == It.IsAny<int>() && eadd.ViewModelName == anyViewModelName)), Times.Once);
        }
    }
}

