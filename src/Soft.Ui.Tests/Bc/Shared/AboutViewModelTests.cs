using Moq;
using Prism.Events;
using Soft.Ui.Bc.Shared;
using Soft.Ui.ViewModel.Services;
using System;
using Xunit;

namespace Soft.Ui.Tests.Bc.Shared
{
    public class AboutViewModelTests
    {
        private AboutViewModel _aboutViewModel;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;

        public AboutViewModelTests()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _aboutViewModel = new AboutViewModel(_eventAggregatorMock.Object, _messageDialogServiceMock.Object);
        }

        [Fact]
        public void TabDisplayName_Returns_About()
        {
            //Assert
            Assert.Equal("About", _aboutViewModel.TabDisplayName);
        }

        [Fact]
        public async void LoadAsync_When_Called_With_Id_0_Returns_True()
        {
            //Act
            var result = await _aboutViewModel.LoadAsync(id:0);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void OnCommandDeleteExecute_Throws_NotImplementedException()
        {
            //Assert
            Assert.Throws<NotImplementedException>(() => _aboutViewModel.CommandDelete.Execute(null));
        }

        [Fact]
        public void OnCommandSaveCanExecute_Throws_NotImplementedException()
        {
            //Assert
            Assert.Throws<NotImplementedException>(() => _aboutViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public void OnCommandSaveExecute_Throws_NotImplementedException()
        {
            //Assert
            Assert.Throws<NotImplementedException>(() => _aboutViewModel.CommandSave.Execute(null));
        }
    }
}
