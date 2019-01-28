using Moq;
using Prism.Events;
using Soft.Model;
using Soft.Ui.Bc.BcCustomer;
using Soft.Ui.Bc.BcBroker;
using Soft.Ui.Shared.Events;
using Soft.Ui.Tests.Extensions;
using Soft.Ui.ViewModel.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Soft.Ui.Tests.Bc.BcBroker
{
    public class BrokerDetailViewModelTests
    {
        private BrokerDetailViewModel _detailViewModelToTest;

        private Mock<IBrokerRepository> _repositoryMock;

        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<EventAfterDetailSaved> _eventAfterDetailSaved;
        private Mock<EventAfterDetailDeleted> _eventAfterDetailDeleted;

        private Mock<IMessageDialogService> _messageDialogServiceMock;

        /// <summary>
        /// Derive an entity test class where Id can be set via constructor for test only
        /// </summary>
        private class CustomerTest : Customer
        {
            public CustomerTest(int id)
            {
                Id = id;
            }
        }
        /// <summary>
        /// Derive an entity test class where Id can be set via constructor for test only
        /// </summary>
        private class BrokerTest : Broker
        {
            public BrokerTest(int id)
            {
                Id = id;
            }
        }
        public BrokerDetailViewModelTests()
        {
            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAfterDetailSaved = new Mock<EventAfterDetailSaved>();
            _eventAfterDetailDeleted = new Mock<EventAfterDetailDeleted>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailSaved>()).Returns(_eventAfterDetailSaved.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailDeleted>()).Returns(_eventAfterDetailDeleted.Object);


            var customern = new List<Customer>
            {
                    new CustomerTest(2) {CustomerNo ="1", Name = "Edward Snowden"}
            };

            var _customernBrokerListe = new List<Broker>
            {
                    new BrokerTest(1) { CustomerNo = "1111", Name = "Broker 1", Customers=customern },
            };

            _repositoryMock = new Mock<IBrokerRepository>();
            _repositoryMock.Setup(dp => dp.FindByIdAsync(1)).ReturnsAsync(_customernBrokerListe.ElementAt(0));

            //_customerDataServiceMock.Setup(dp => dp.SaveAsync(It.IsAny<Customer>())).Callback().Returns(True);

            _detailViewModelToTest = new BrokerDetailViewModel(_eventAggregatorMock.Object,
                                                                  _messageDialogServiceMock.Object,
                                                                  _repositoryMock.Object);
        }

        [Fact]
        public async void LoadAsync_When_Called_With_Id_Should_Load_Broker_By_Calling_Repository_GetByIdAsync()
        {
            //Arrange
            var irgendeineExistierendeId = 1;

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);

            //Assert
            Assert.NotNull(_detailViewModelToTest.CustomerBrokerWrapper);
            Assert.Equal(irgendeineExistierendeId, _detailViewModelToTest.CustomerBrokerWrapper.Id);

            _repositoryMock.Verify(dp => dp.FindByIdAsync(irgendeineExistierendeId), Times.Once);
        }
        [Fact]
        public async void LoadAsync_When_Called_With_0_Should_Create_New_Broker_To_Repository_And_Should_Not_Call_Repository_GetByIdAsync()
        {
            //Act
            await _detailViewModelToTest.LoadAsync(0);

            //Assert
            Assert.NotNull(_detailViewModelToTest.CustomerBrokerWrapper);
            Assert.Equal(0, _detailViewModelToTest.CustomerBrokerWrapper.Id);

            _repositoryMock.Verify(dp => dp.FindByIdAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(dp => dp.Add(_detailViewModelToTest.CustomerBrokerWrapper.Entity), Times.Once);
        }
        [Fact]
        public void LoadAsync_When_Called_Should_Raise_PropertyChanged_For_CustomerBrokerWrapper_Property()
        {
            // Arrange
            var irgendeineExistierendeId = 1;

            //Act
            var fired = _detailViewModelToTest.IsPropertyChangedFired(
              async () => await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId),
              nameof(_detailViewModelToTest.CustomerBrokerWrapper));

            //Assert
            Assert.True(fired);
        }
        [Fact]
        public void HasChanges_When_Set_To_Different_Value_Should_Raise_PropertyChanged()
        {
            //Act
            _detailViewModelToTest.HasChanges = false;
            var fired = _detailViewModelToTest.IsPropertyChangedFired(
              () => _detailViewModelToTest.HasChanges = true,
              nameof(_detailViewModelToTest.HasChanges));

            //Assert
            Assert.True(fired);
        }
        [Fact]
        public void HasChanges_When_Set_To_Same_Value_Should_Not_Raise_PropertyChanged()
        {
            //Act
            _detailViewModelToTest.HasChanges = true;
            var fired = _detailViewModelToTest.IsPropertyChangedFired(
              () => _detailViewModelToTest.HasChanges = true,
              nameof(_detailViewModelToTest.HasChanges));

            //Assert
            Assert.False(fired);
        }
        [Fact]
        public async void CommandSave_When_Called_For_Loaded_Customer_Should_Call_Repository_SaveAsync_For_Customer()
        {
            // Arrange
            var irgendeineExistierendeCustomernId = 1;

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeCustomernId);
            _detailViewModelToTest.CommandSave.Execute(null);

            //Assert
            _repositoryMock.Verify(kdsm => kdsm.SaveAsync(), Times.Once);
        }
        [Fact]
        public async void CommandSave_When_Called_For_Loaded_Customer_Should_Publish_EventAfterSavedCustomer_For_Customer()
        {
            // Arrange
            var irgendeineExistierendeCustomernId = 1;

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeCustomernId);
            _detailViewModelToTest.CommandSave.Execute(null);

            //Assert
            _eventAfterDetailSaved.Verify(akse => akse.Publish(It.Is<EventAfterDetailSavedArgs>
                (aksea => aksea.Id == irgendeineExistierendeCustomernId)), Times.Once);
            //(aksea => aksea.Id == irgendeineExistierendeCustomernId && aksea.DisplayMember == _detailViewModelToTest.Customer.Name)), Times.Once);
        }
        [Fact]
        public async void CommandDelete_When_Called_And_Confirmed_Should_Call_Repository_Remove_For_Broker_And_SaveAsync()
        {
            // Arrange
            var irgendeineExistierendeId = 1;
            _messageDialogServiceMock.Setup(mdsm =>
                            mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.OK);

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);
            _detailViewModelToTest.CommandDelete.Execute(null);

            //Assert
            _repositoryMock.Verify(kdsm => kdsm.Remove(_detailViewModelToTest.CustomerBrokerWrapper.Entity), Times.Once);
            _repositoryMock.Verify(kdsm => kdsm.SaveAsync(), Times.Once);
        }
        [Fact]
        public async void CommandDelete_When_Called_For_Loaded_Broker_Should_Call_Repository_Remove_For_Broker_And_SaveAsync()
        {
            // Arrange
            var irgendeineExistierendeId = 1;
            _messageDialogServiceMock.Setup(mdsm =>
                mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.Cancel);

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);
            _detailViewModelToTest.CommandDelete.Execute(null);

            //Assert
            _repositoryMock.Verify(kdsm => kdsm.Remove(_detailViewModelToTest.CustomerBrokerWrapper.Entity), Times.Never);
            _repositoryMock.Verify(kdsm => kdsm.SaveAsync(), Times.Never);
        }
        [Fact]
        public async void CommandDelete_When_Called_And_Confirmed_Should_Publish_EventAfterDetailDeleted_For_Broker()
        {
            // Arrange
            var irgendeineExistierendeId = 1;
            _messageDialogServiceMock.Setup(mdsm =>
                mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.OK);

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);
            _detailViewModelToTest.CommandDelete.Execute(null);

            //Assert
            _eventAfterDetailDeleted.Verify(akse => akse.Publish(It.Is<EventAfterDetailDeletedArgs>
                (eadd => eadd.Id == irgendeineExistierendeId && eadd.ViewModelName == nameof(BrokerDetailViewModel))),
                Times.Once);
        }
        [Fact]
        public async void CommandDelete_When_Called_And_Canceled_Should_Not_Publish_EventAfterDetailDeleted_For_Broker()
        {
            // Arrange
            var irgendeineExistierendeId = 1;
            _messageDialogServiceMock.Setup(mdsm =>
                mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.Cancel);

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);
            _detailViewModelToTest.CommandDelete.Execute(null);

            //Assert
            _eventAfterDetailDeleted.Verify(akse => akse.Publish(
                new EventAfterDetailDeletedArgs
                {
                    Id = irgendeineExistierendeId,
                    ViewModelName = nameof(CustomerDetailViewModel)
                }
                ), Times.Never);
        }
        [Fact]
        public async void CommandSave_When_CustomerBrokerDetailViewModel_Broker_Name_Has_Changes_And_Errors_Should_Be_Disabled()
        {
            // Arrange
            var irgendeineExistierendeId = 1;
            _repositoryMock.Setup(dp => dp.HasChanges()).Returns(true);

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);
            Assert.False(_detailViewModelToTest.HasChanges, "HasChanges sollte false sein");
            _detailViewModelToTest.CustomerBrokerWrapper.Name = ".";

            //Assert
            Assert.True(_detailViewModelToTest.HasChanges, "HasChanges sollte true sein");
            Assert.True(_detailViewModelToTest.CustomerBrokerWrapper.HasErrors, "HasErrors sollte true sein");
            Assert.False(_detailViewModelToTest.CommandSave.CanExecute(null));
        }
        [Fact]
        public async void CommandSave_When_CustomerBrokerDetailViewModel_Broker_Name_Has_Changes_And_No_Errors_Should_Be_Enabled()
        {
            // Arrange
            var irgendeineExistierendeId = 1;
            _repositoryMock.Setup(dp => dp.HasChanges()).Returns(true);

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);
            Assert.False(_detailViewModelToTest.HasChanges, "HasChanges sollte false sein");
            _detailViewModelToTest.CustomerBrokerWrapper.Name = "Vorname NeuerNachname";

            //Assert
            Assert.True(_detailViewModelToTest.HasChanges, "HasChanges sollte true sein");
            Assert.False(_detailViewModelToTest.CustomerBrokerWrapper.HasErrors, "HasErrors sollte false sein");
            Assert.True(_detailViewModelToTest.CommandSave.CanExecute(null));
        }
        [Fact]
        public void CommandSave_When_CustomerBrokerDetailViewModel_Has_No_Broker_Set_Should_Be_Disabled()
        {
            //Assert
            Assert.False(_detailViewModelToTest.CommandSave.CanExecute(null));
        }
        [Fact]
        public async void CommandSave_When_After_CommandSave_Is_Executed_Should_Be_Disabled()
        {
            // Arrange
            var irgendeineExistierendeId = 1;
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);
            Assert.False(_detailViewModelToTest.HasChanges, "HasChanges sollte false sein");

            //Act
            //Davon ausgehen, dass das Repository bei einer Änderung true zurückliefern würde
            _repositoryMock.Setup(dp => dp.HasChanges()).Returns(true);
            _detailViewModelToTest.CustomerBrokerWrapper.Name = "Vorname NeuerNachname";

            //Davon ausgehen, dass das Repository nach dem Speichern false zurückliefern würde
            _repositoryMock.Setup(dp => dp.HasChanges()).Returns(false);
            _detailViewModelToTest.CommandSave.Execute(null);

            //Assert
            Assert.False(_detailViewModelToTest.CommandSave.CanExecute(null));
        }
        [Fact]
        public async void CommandSave_When_Broker_Is_Initialy_Loaded_Should_Be_Disabled()
        {
            // Arrange
            var irgendeineExistierendeId = 1;

            //Act
            await _detailViewModelToTest.LoadAsync(irgendeineExistierendeId);

            //Assert
            Assert.False(_detailViewModelToTest.CommandSave.CanExecute(null));
        }

    }
}
