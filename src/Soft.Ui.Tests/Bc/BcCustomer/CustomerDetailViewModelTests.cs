using Moq;
using Prism.Events;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using Soft.Model.Shared;
using Soft.Ui.Bc.BcAccountManager;
using Soft.Ui.Bc.BcCustomer;
using Soft.Ui.Shared.Events;
using Soft.Ui.Tests.Extensions;
using Soft.Ui.ViewModel.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Soft.Ui.Tests.Bc.BcCustomer
{
    public class CustomerDetailViewModelTests
    {
        private CustomerDetailViewModel _customerDetailViewModel;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<IGenericLookupDataService<AccountManager>> _accountManagerLookupDataServiceMock;
        private List<LookupItem> _lookupItemAccountManagerList;

        private Mock<EventAfterDetailSaved> _eventAfterDetailSavedMock;
        private Mock<EventAfterDetailDeleted> _eventAfterDetailDeletedMock;
        private Mock<EventAfterDetailClose> _eventAfterDetailCloseMock;
        private List<Customer> _customerList;
        private List<BankAccount> _bankAccountsCustomerId1;
        private List<BankAccount> _bankAccountsCustomerId2;

        public CustomerDetailViewModelTests()
        {
            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            _accountManagerLookupDataServiceMock = new Mock<IGenericLookupDataService<AccountManager>>();
            _lookupItemAccountManagerList = new List<LookupItem>
                {
                    new LookupItem {Id=1, DisplayMember = "Account Manager1" },
                    new LookupItem {Id=2, DisplayMember = "Account Manager2" },
                    new LookupItem {Id=3, DisplayMember = "Account Manager3" }
                };
            _accountManagerLookupDataServiceMock.Setup(dp => dp.GetEntityLookupAsync()).ReturnsAsync(_lookupItemAccountManagerList);

            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAfterDetailSavedMock = new Mock<EventAfterDetailSaved>();
            _eventAfterDetailDeletedMock = new Mock<EventAfterDetailDeleted>();
            _eventAfterDetailCloseMock = new Mock<EventAfterDetailClose>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailSaved>()).Returns(_eventAfterDetailSavedMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailDeleted>()).Returns(_eventAfterDetailDeletedMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailClose>()).Returns(_eventAfterDetailCloseMock.Object);

            var accountManager = new AccountManager();
            _bankAccountsCustomerId1 = new List<BankAccount>
                {
                    new BankAccount{Id=1, AccountNo ="1012345678", CustomerId=1},
                    new BankAccount{Id=2, AccountNo ="1801234567", CustomerId=1}
                };
            _bankAccountsCustomerId2 = new List<BankAccount>
                {
                    new BankAccount{Id=2, AccountNo ="2012345678", CustomerId=2}
                };
            _customerList = new List<Customer>
                {
                    new Customer {Id=1, Name = "Bill Gates", AccountManager=accountManager, BankAccounts=_bankAccountsCustomerId1 },
                    new Customer {Id=2, Name = "Edward Snowden", AccountManager=accountManager, BankAccounts=_bankAccountsCustomerId2 }
                };

            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerRepositoryMock.Setup(dp => dp.FindByIdAsync(1)).ReturnsAsync(_customerList.ElementAt(0));
            _customerRepositoryMock.Setup(dp => dp.FindByIdAsync(2)).ReturnsAsync(_customerList.ElementAt(1));

            //_customerDataServiceMock.Setup(dp => dp.SaveAsync(It.IsAny<Customer>())).Callback().Returns(True);
            //_customerRepositoryMock.Setup(dp => dp.SaveAsync()).Returns(Task.FromResult(CustomerRepository.MsgSaveSuccessful));

            _customerDetailViewModel = new CustomerDetailViewModel(_customerRepositoryMock.Object,
                                                                   _eventAggregatorMock.Object,
                                                                   _messageDialogServiceMock.Object,
                                                                   _accountManagerLookupDataServiceMock.Object);
        }

        [Fact]
        public async void LoadAsync_When_Called_With_Customer_Id_Should_Set_Id_To_Customer_Id()
        {
            //Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);

            //Assert
            Assert.Equal(anyInTheMockDefinedCustomerId, _customerDetailViewModel.Customer.Id);
        }

        [Fact]
        public async void LoadAsync_When_Called_With_0_Should_Set_Id_To_0()
        {
            //Act
            await _customerDetailViewModel.LoadAsync(customerId: 0);

            //Assert
            Assert.Equal(0, _customerDetailViewModel.Customer.Id);
        }
        [Fact]
        public async void LoadAsync_When_Called_With_Customer_Id_Should_Load_Customer_By_Calling_Repository_FindByIdAsync()
        {
            //Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);

            //Assert
            Assert.NotNull(_customerDetailViewModel.Customer);
            _customerRepositoryMock.Verify(dp => dp.FindByIdAsync(anyInTheMockDefinedCustomerId), Times.Once);
        }
        [Fact]
        public async void LoadAsync_When_Called_With_0_Should_Create_New_Customer_In_Repository_And_Should_Not_Call_Repository_FindByIdAsync()
        {
            //Act
            await _customerDetailViewModel.LoadAsync(customerId: 0);

            //Assert
            Assert.NotNull(_customerDetailViewModel.Customer);
            Assert.Equal(System.DateTime.Now.Date, _customerDetailViewModel.Customer.CustomerSince.Date);

            _customerRepositoryMock.Verify(dp => dp.FindByIdAsync(It.IsAny<int>()), Times.Never);
            _customerRepositoryMock.Verify(dp => dp.Add(_customerDetailViewModel.Customer.Entity), Times.Once);
        }
        [Fact]
        public void LoadAsync_When_Called_Should_Raise_PropertyChanged_For_Property_Customer()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            var fired = _customerDetailViewModel.IsPropertyChangedFired(
              async () => await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId),
              nameof(_customerDetailViewModel.Customer));

            //Assert
            Assert.True(fired);
        }
        [Fact]
        public async void LoadAsync_When_Called_For_A_New_Customer_Should_Trigger_Validation_By_Emptying_Property_Customer_Name()
        {
            //Act 
            //New Customer has customerId=0
            await _customerDetailViewModel.LoadAsync(customerId: 0);

            //Assert
            Assert.Equal("", _customerDetailViewModel.Customer.Name);
        }
        [Fact]
        public async void LoadAsync_When_Called_Should_Set_Customer_BankAccounts()
        {
            //Arrange
            var inTheMockDefinedCustomerWithBankAccounts = _customerList.FirstOrDefault(c => c.BankAccounts.Count > 0);
            Assert.NotNull(inTheMockDefinedCustomerWithBankAccounts);

            var expectedNumberOfCustomerBankAccounts = inTheMockDefinedCustomerWithBankAccounts.BankAccounts.Count();

            //Act
            await _customerDetailViewModel.LoadAsync(inTheMockDefinedCustomerWithBankAccounts.Id);

            //Assert
            Assert.Equal(expectedNumberOfCustomerBankAccounts, _customerDetailViewModel.ListCustomerBankAccountWrapper.Count());
        }
        [Fact]
        public async void LoadAsync_When_Called_Twice_Should_Set_Customer_BankAccounts_Only_Once()
        {
            //Arrange
            var inTheMockDefinedCustomerWithBankAccounts = _customerList.FirstOrDefault(c => c.BankAccounts.Count > 0);
            Assert.NotNull(inTheMockDefinedCustomerWithBankAccounts);

            var expectedNumberOfCustomerBankAccounts = inTheMockDefinedCustomerWithBankAccounts.BankAccounts.Count();

            //Act
            await _customerDetailViewModel.LoadAsync(inTheMockDefinedCustomerWithBankAccounts.Id);
            await _customerDetailViewModel.LoadAsync(inTheMockDefinedCustomerWithBankAccounts.Id);

            //Assert
            Assert.Equal(expectedNumberOfCustomerBankAccounts, _customerDetailViewModel.ListCustomerBankAccountWrapper.Count());
        }
        [Fact]
        public async void LoadAsync_When_Called_Twice_Should_Set_AccountManagerLookupItemList_Only_Once()
        {
            //Arrange
            var anyInTheMockDefinedCustomerId = 2;
            var expectedNumberOfAccountManagerLookupItems = _lookupItemAccountManagerList.Count() + 1;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);

            //Assert
            Assert.Equal(expectedNumberOfAccountManagerLookupItems, _customerDetailViewModel.LookupItemsAccountManager.Count());
        }
        [Fact]
        public async void LoadAsync_When_Called_With_None_Existing_CustomerId_Should_Return_False()
        {
            //Arrange
            var anyNotInTheMockDefinedCustomerId = 100000000;

            //Act
            var result = await _customerDetailViewModel.LoadAsync(anyNotInTheMockDefinedCustomerId);

            //Assert
            Assert.False(result);
        }
        [Fact]
        public void HasChanges_When_Set_From_False_To_True_Should_Raise_PropertyChanged_Of_HasChanges()
        {
            //Act
            _customerDetailViewModel.HasChanges = false;
            var fired = _customerDetailViewModel.IsPropertyChangedFired(
              () => _customerDetailViewModel.HasChanges = true,
              nameof(_customerDetailViewModel.HasChanges));

            //Assert
            Assert.True(fired);
        }
        [Fact]
        public void HasChanges_When_Set_To_Same_Value_Should_Not_Raise_PropertyChanged_Of_HasChanges()
        {
            //Act
            _customerDetailViewModel.HasChanges = true;
            var fired = _customerDetailViewModel.IsPropertyChangedFired(
              () => _customerDetailViewModel.HasChanges = true,
              nameof(_customerDetailViewModel.HasChanges));

            //Assert
            Assert.False(fired);
        }
        [Fact]
        public async void CommandSave_After_Customer_Loaded_Should_Call_Customer_Repository_SaveAsync_Once()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            _customerDetailViewModel.CommandSave.Execute(null);

            //Assert
            _customerRepositoryMock.Verify(crm => crm.SaveAsync(), Times.Once);
        }
        [Fact]
        public async void CommandSave_When_SaveAsync_Throws_DbUpdateException_Should_Display_MsgService_With_SaveFailed_Msg()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            _customerRepositoryMock.Setup(crm => crm.SaveAsync()).ThrowsAsync(new DbUpdateException());
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);

            _customerDetailViewModel.CommandSave.Execute(null);

            //Assert
            _messageDialogServiceMock.Verify(m => m.ShowInfoDialog("Entity can not be deleted as it is referenced by another Entity. Reference has to be removed before.", "Save failed"), Times.Once);
            //else part in OnCommandSaveExecute must not be executed:
            _customerRepositoryMock.Verify(crm => crm.HasChanges(), Times.Never);
            _eventAfterDetailSavedMock.Verify(eadsm => eadsm.Publish(It.Is<EventAfterDetailSavedArgs>
                (eadsa => eadsa.Id == anyInTheMockDefinedCustomerId)), Times.Never);
        }
        [Fact]
        public async void CommandSave_When_SaveAsync_Throws_DbUpdateConcurrencyException_Should_Display_MsgService_With_SaveFailed_Msg()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            _customerRepositoryMock.Setup(crm => crm.SaveAsync()).ThrowsAsync(new DbUpdateConcurrencyException());
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);

            _customerDetailViewModel.CommandSave.Execute(null);

            //Assert
            _messageDialogServiceMock.Verify(m => m.ShowInfoDialog("Customer data can not be saved, as data was changed in the meanwhile !", "Save failed"), Times.Once);
            //else part in OnCommandSaveExecute must not be executed:
            _customerRepositoryMock.Verify(crm => crm.HasChanges(), Times.Never);
            _eventAfterDetailSavedMock.Verify(eadsm => eadsm.Publish(It.Is<EventAfterDetailSavedArgs>
                (eadsa => eadsa.Id == anyInTheMockDefinedCustomerId)), Times.Never);
        }
        [Fact]
        public async void CommandSave_After_Customer_Loaded_And_No_Changes_Applied_Should_Be_Disabled()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);

            //Assert
            Assert.False(_customerDetailViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public async void CommandSave_After_Customer_Loaded_Should_Set_Id_To_Customer_Id()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            _customerDetailViewModel.CommandSave.Execute(null);

            //Assert
            Assert.Equal(anyInTheMockDefinedCustomerId, _customerDetailViewModel.Id);
        }

        //[Fact]
        //public async void CommandSave_When_Called_For_New_Customer_With_Id_0_Should_Set_Id_Greater_0()
        //{
        //    //How to test ? 
        //    //CommandSave -> _repository.SaveAsync() -> Customer.Id wird > 0 
        //    //Wie mockt man "Customer.Id wird > 0" ?

        //    // Arrange
        //    int? newCustomer_Id_Null = null;

        //    //Act
        //    await _customerDetailViewModel.LoadAsync(newCustomer_Id_Null);
        //    _customerDetailViewModel.CommandSave.Execute(null);

        //    //Assert
        //    Assert.True(_customerDetailViewModel.Customer.Id > 0);
        //    Assert.True(_customerDetailViewModel.Id > 0);
        //}

        [Fact]
        public async void CommandSave_After_Customer_Loaded_Should_Publish_EventAfterDetailSaved_For_Customer()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            _customerDetailViewModel.CommandSave.Execute(null);

            //Assert
            _eventAfterDetailSavedMock.Verify(eadsm => eadsm.Publish(It.Is<EventAfterDetailSavedArgs>
                (eadsa => eadsa.Id == anyInTheMockDefinedCustomerId)), Times.Once);
        }

        [Fact]
        public async void CommandSave_When_CustomerDetailViewModel_Customer_Name_Has_Changes_And_Errors_Should_Be_Disabled()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            _customerRepositoryMock.Setup(crm => crm.HasChanges()).Returns(true);

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            Assert.False(_customerDetailViewModel.HasChanges, "HasChanges must be false");
            //Set customer name so that validation fails:
            _customerDetailViewModel.Customer.Name = ".";

            //Assert
            Assert.True(_customerDetailViewModel.HasChanges, "HasChanges must be true");
            Assert.True(_customerDetailViewModel.Customer.HasErrors, "HasErrors must be true");
            Assert.False(_customerDetailViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public async void CommandSave_When_CustomerDetailViewModel_Customer_Name_Has_Changes_And_No_Errors_Should_Be_Enabled()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            _customerRepositoryMock.Setup(crm => crm.HasChanges()).Returns(true);

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            Assert.False(_customerDetailViewModel.HasChanges, "HasChanges must be false");
            _customerDetailViewModel.Customer.Name = "Surname NewLastName";

            //Assert
            Assert.True(_customerDetailViewModel.HasChanges, "HasChanges must be true");
            Assert.False(_customerDetailViewModel.Customer.HasErrors, "HasErrors must be false");
            Assert.True(_customerDetailViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public async void CommandSave_When_Customer_BankAccount_AccountNo_Set_To_Invalid_Null_Value_Should_Be_Disabled()
        {
            //Arrange
            var inTheMockDefinedCustomerWithBankAccounts = _customerList.FirstOrDefault(c => c.BankAccounts.Count > 0);
            Assert.NotNull(inTheMockDefinedCustomerWithBankAccounts);

            _customerRepositoryMock.Setup(crm => crm.HasChanges()).Returns(true);

            //Act
            await _customerDetailViewModel.LoadAsync(inTheMockDefinedCustomerWithBankAccounts.Id);
            Assert.False(_customerDetailViewModel.HasChanges, "HasChanges must be false");

            var customerBankAccountWrapper = _customerDetailViewModel.ListCustomerBankAccountWrapper.FirstOrDefault();
            customerBankAccountWrapper.AccountNo = null;

            //Assert
            Assert.True(_customerDetailViewModel.HasChanges, "HasChanges must be true");
            Assert.True(customerBankAccountWrapper.HasErrors, "HasErrors must be true");
            Assert.False(_customerDetailViewModel.CommandSave.CanExecute(null), "CommandSave must be disabled on validation errors");
        }

        [Fact]
        public void CommandSave_When_CustomerDetailViewModel_Has_No_Customer_Set_Should_Be_Disabled()
        {
            //Assert
            Assert.False(_customerDetailViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public async void CommandSave_After_CommandSave_Is_Executed_Should_Be_Disabled()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            Assert.False(_customerDetailViewModel.HasChanges, "HasChanges must be false");

            //Act
            //Mock repository.HasChanges() returns true
            _customerRepositoryMock.Setup(crm => crm.HasChanges()).Returns(true);
            _customerDetailViewModel.Customer.Name = "Surname NewLastname";

            //Mock repository.HasChanges() returns false
            _customerRepositoryMock.Setup(dp => dp.HasChanges()).Returns(false);
            _customerDetailViewModel.CommandSave.Execute(null);

            //Assert
            Assert.False(_customerDetailViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public async void CommandSave_When_Customer_Is_Initialy_Loaded_Should_Be_Disabled()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);

            //Assert
            Assert.False(_customerDetailViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public async void CommandDelete_When_Called_And_Confirmed_Should_Call_Repository_Remove_For_Customer_And_SaveAsync()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            _messageDialogServiceMock.Setup(mdsm =>
                            mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.OK);

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            _customerDetailViewModel.CommandDelete.Execute(null);

            //Assert
            _customerRepositoryMock.Verify(crm => crm.Remove(_customerDetailViewModel.Customer.Entity), Times.Once);
            _customerRepositoryMock.Verify(crm => crm.SaveAsync(), Times.Once);
        }

        [Fact]
        public async void CommandDelete_When_Called_After_Customer_Loaded_Should_Call_Repository_Remove_For_Customer_And_SaveAsync()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            _messageDialogServiceMock.Setup(mdsm =>
                mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.Cancel);

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            _customerDetailViewModel.CommandDelete.Execute(null);

            //Assert
            _customerRepositoryMock.Verify(crm => crm.Remove(_customerDetailViewModel.Customer.Entity), Times.Never);
            _customerRepositoryMock.Verify(crm => crm.SaveAsync(), Times.Never);
        }

        [Fact]
        public async void CommandDelete_When_Called_And_Confirmed_Should_Publish_EventAfterDeletedCustomer_For_Customer()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            _messageDialogServiceMock.Setup(mdsm =>
                mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.OK);

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            _customerDetailViewModel.CommandDelete.Execute(null);

            //Assert
            _eventAfterDetailDeletedMock.Verify(eaddm => eaddm.Publish(It.Is<EventAfterDetailDeletedArgs>
                (eadda => eadda.Id == anyInTheMockDefinedCustomerId && eadda.ViewModelName == nameof(CustomerDetailViewModel))),
                Times.Once);
        }

        [Fact]
        public async void CommandCloseDetailView_When_Called_And_DetailViewModel_Has_No_Changes_Should_Publish_EventCloseDetailView()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            //_customerDetailViewModel.HasChanges = false; // nicht notwendig da keine Änderungen erfolgt
            _customerDetailViewModel.CommandClose.Execute(null);

            //Assert
            _eventAfterDetailCloseMock.Verify(
                akse => akse.Publish(It.Is<EventAfterDetailCloseArgs>
                (aksea => aksea.Id == anyInTheMockDefinedCustomerId
                       && aksea.ViewModelName == _customerDetailViewModel.GetType().Name)),
                Times.Once);
        }

        [Theory]
        [InlineData(MessageDialogResult.Cancel, 0)]
        [InlineData(MessageDialogResult.OK, 1)]
        public async void CommandCloseDetailView_When_Called_And_DetailViewModel_Has_Changes_Should_Publish_EventCloseDetailView_Depending_On_MessageDialogResult
            (
            MessageDialogResult messageDialogResult,
            int expexctedPublishCallsEventAfterDetailClose
            )
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            _messageDialogServiceMock.Setup(mdsm => mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(messageDialogResult);

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            _customerDetailViewModel.HasChanges = true;
            _customerDetailViewModel.CommandClose.Execute(null);

            //Assert
            _eventAfterDetailCloseMock.Verify(
                 akse => akse.Publish(It.Is<EventAfterDetailCloseArgs>
                 (aksea => aksea.Id == anyInTheMockDefinedCustomerId
                        && aksea.ViewModelName == _customerDetailViewModel.GetType().Name)),
                 Times.Exactly(expexctedPublishCallsEventAfterDetailClose));
        }

        [Fact]
        public async void Customer_When_Set_Should_Enable_CommandDelete()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);

            //Assert
            Assert.True(_customerDetailViewModel.CommandDelete.CanExecute(null));
        }

        [Fact]
        public void Ctor_When_Called_Should_Initialize_LookupItemsAccountManager()
        {
            Assert.NotNull(_customerDetailViewModel.LookupItemsAccountManager);
        }

        [Fact]
        public void Ctor_When_Called_Should_Initialize_ListCustomerBankAccountWrapper()
        {
            Assert.NotNull(_customerDetailViewModel.ListCustomerBankAccountWrapper);
        }

        [Fact]
        public void SelectedBankAccount_When_Set_To_New_CustomerBankAccountWrapper_Should_Raise_PropertyChanged()
        {
            //Act
            var fired = _customerDetailViewModel.IsPropertyChangedFired(
                () => _customerDetailViewModel.SelectedBankAccount = new CustomerBankAccountWrapper(new BankAccount()),
                nameof(_customerDetailViewModel.SelectedBankAccount));

            //Assert
            Assert.True(fired);
        }

        [Fact]
        public void SelectedBankAccount_When_Set_To_New_CustomerBankAccountWrapper_Should_Enable_CommandDeleteBankAccount()
        {
            //Act
            _customerDetailViewModel.SelectedBankAccount = new CustomerBankAccountWrapper(new BankAccount());

            //Assert
            Assert.True(_customerDetailViewModel.CommandDeleteBankAccount.CanExecute(null));
        }

        [Fact]
        public void SelectedBankAccount_When_Set_To_Null_Should_Disable_CommandDeleteBankAccount()
        {
            //Act
            _customerDetailViewModel.SelectedBankAccount = null;

            //Assert
            Assert.False(_customerDetailViewModel.CommandDeleteBankAccount.CanExecute(null));
        }

        [Fact]
        public async void CommandAddBankAccount_When_Called_Should_Add_New_CustomerBankAccountWrapper_To_ListCustomerBankAccountWrapper()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            var listCustomerBankAccountWrapperCountBeforeCommand = _customerDetailViewModel.ListCustomerBankAccountWrapper.Count();
            var expectedCount = 1 + listCustomerBankAccountWrapperCountBeforeCommand;

            //Act  
            _customerDetailViewModel.CommandAddBankAccount.Execute(null);

            //Assert
            Assert.Equal(expectedCount, _customerDetailViewModel.ListCustomerBankAccountWrapper.Count());
        }

        [Fact]
        public async void CommandAddBankAccount_When_Called_Should_Add_New_BankAccount_To_Customer_Model_BankAccounts()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            var customerBankAccounts = _customerDetailViewModel.Customer.Entity.BankAccounts.Count();
            var expectedCount = 1 + customerBankAccounts;

            //Act  
            _customerDetailViewModel.CommandAddBankAccount.Execute(null);

            //Assert
            Assert.Equal(expectedCount, _customerDetailViewModel.Customer.Entity.BankAccounts.Count());
        }

        [Fact]
        public async void CommandDeleteBankAccount_When_Called_And_Confirmed_Should_Remove_SelectedBankAccount_From_ListCustomerBankAccountWrapper_And_Set_To_Null()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            //Mock Dialog OK Action:
            _messageDialogServiceMock.Setup(mdsm =>
                            mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.OK);

            //To remove a Bank Account add a new Bank Account first:
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            var newCustomerBankAccountWrapper = new CustomerBankAccountWrapper(new BankAccount());
            _customerDetailViewModel.ListCustomerBankAccountWrapper.Add(newCustomerBankAccountWrapper);

            Assert.Contains(newCustomerBankAccountWrapper, _customerDetailViewModel.ListCustomerBankAccountWrapper);

            //Act
            _customerDetailViewModel.SelectedBankAccount = newCustomerBankAccountWrapper;
            _customerDetailViewModel.CommandDeleteBankAccount.Execute(null);

            //Assert
            Assert.DoesNotContain(newCustomerBankAccountWrapper, _customerDetailViewModel.ListCustomerBankAccountWrapper);
            Assert.Null(_customerDetailViewModel.SelectedBankAccount);
        }

        [Fact]
        public async void CommandDeleteBankAccount_When_Called_And_Canceled_Should_Neither_Change_SelectedBankAccount_Nor_Remove_It_From_ListCustomerBankAccountWrapper()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            //Mock Dialog Cancel Action:
            _messageDialogServiceMock.Setup(mdsm =>
                            mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.Cancel);

            //To remove a Bank Account add a new Bank Account first:
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            var newCustomerBankAccountWrapper = new CustomerBankAccountWrapper(new BankAccount());
            _customerDetailViewModel.ListCustomerBankAccountWrapper.Add(newCustomerBankAccountWrapper);

            Assert.Contains(newCustomerBankAccountWrapper, _customerDetailViewModel.ListCustomerBankAccountWrapper);

            //Act
            _customerDetailViewModel.SelectedBankAccount = newCustomerBankAccountWrapper;
            _customerDetailViewModel.CommandDeleteBankAccount.Execute(null);

            //Assert
            Assert.Contains(newCustomerBankAccountWrapper, _customerDetailViewModel.ListCustomerBankAccountWrapper);
            Assert.Equal(newCustomerBankAccountWrapper, _customerDetailViewModel.SelectedBankAccount);
        }

        [Fact]
        public async void CommandDeleteBankAccount_When_Called_And_Confirmed_Should_Call_Repository_RemoveBankAccount_For_SelectedBankAccount()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            //Mock Dialog OK Action:
            _messageDialogServiceMock.Setup(mdsm =>
            mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.OK);

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            _customerDetailViewModel.CommandAddBankAccount.Execute(null);

            _customerDetailViewModel.SelectedBankAccount = _customerDetailViewModel.ListCustomerBankAccountWrapper.FirstOrDefault();
            var selectedBankAccount = _customerDetailViewModel.SelectedBankAccount.Entity;
            _customerDetailViewModel.CommandDeleteBankAccount.Execute(null);

            //Assert
            _customerRepositoryMock.Verify(crm => crm.RemoveBankAccount(selectedBankAccount), Times.Once);
        }

        [Fact]
        public async void CommandDeleteBankAccount_When_Called_And_Canceled_Should_Not_Remove_SelectedBankAccount_From_Customer_Model_BankAccounts()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;
            //Mock Dialog Cancel Action:
            _messageDialogServiceMock.Setup(mdsm =>
                            mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.Cancel);

            //To remove a Bank Account add a new Bank Account first:
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            var expectedCountBankAccounts = _customerDetailViewModel.Customer.Entity.BankAccounts.Count();
            _customerDetailViewModel.CommandAddBankAccount.Execute(null);
            Assert.Equal(expectedCountBankAccounts + 1, _customerDetailViewModel.Customer.Entity.BankAccounts.Count());

            //Act
            _customerDetailViewModel.SelectedBankAccount = _customerDetailViewModel.ListCustomerBankAccountWrapper.FirstOrDefault();
            _customerDetailViewModel.CommandDeleteBankAccount.Execute(null);

            //Assert
            Assert.Equal(expectedCountBankAccounts + 1, _customerDetailViewModel.Customer.Entity.BankAccounts.Count());
        }

        [Fact]
        public async void CommandDeleteBankAccount_When_Called_And_Confirmed_Should_Set_HasChanges_And_Enable_CommandSave()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 1;
            //Mock Dialog OK Action:
            _messageDialogServiceMock.Setup(mdsm =>
                            mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.OK);
            _customerRepositoryMock.Setup(crm => crm.HasChanges()).Returns(true);

            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            Assert.True(_customerDetailViewModel.Customer.Entity.BankAccounts.Count() > 0, "For this test Customer must have at least 1 Bank Account");

            _customerDetailViewModel.SelectedBankAccount = _customerDetailViewModel.ListCustomerBankAccountWrapper.FirstOrDefault();
            
            //Act
            _customerDetailViewModel.CommandDeleteBankAccount.Execute(null);

            //Assert
            Assert.True(_customerDetailViewModel.HasChanges, "HasChanges must be true");
            Assert.False(_customerDetailViewModel.Customer.HasErrors, "HasErrors must be false");
            Assert.True(_customerDetailViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public async void CommandDeleteBankAccount_When_Called_And_Canceled_Should_Not_Set_HasChanges_And_Not_Enable_CommandSave()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 1;
            _messageDialogServiceMock.Setup(mdsm =>
                            mdsm.ShowOkCancelDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(MessageDialogResult.Cancel);
            _customerRepositoryMock.Setup(dp => dp.HasChanges()).Returns(true);

            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            Assert.True(_customerDetailViewModel.Customer.Entity.BankAccounts.Count() > 0, "For this test Customer must have at least 1 Bank Account");

            _customerDetailViewModel.SelectedBankAccount = _customerDetailViewModel.ListCustomerBankAccountWrapper.FirstOrDefault();

            //Act
            _customerDetailViewModel.CommandDeleteBankAccount.Execute(null);

            //Assert
            Assert.False(_customerDetailViewModel.HasChanges, "HasChanges must be true");
            Assert.False(_customerDetailViewModel.Customer.HasErrors, "HasErrors must be false");
            Assert.False(_customerDetailViewModel.CommandSave.CanExecute(null));
        }

        [Fact]
        public void Name_Getter_When_Called_Should_Return_NameOf_CustomerDetailViewModel()
        {
            Assert.Equal(nameof(CustomerDetailViewModel), _customerDetailViewModel.Name);
        }

        [Fact]
        public async void EventAfterDetailSaved_When_Called_From_AccountManagerDetailViewModel_Should_Update_ListAccountManager()
        {
            //Arrange
            var anyInTheMockDefinedCustomerId = 2;
            var eventAfterDetailSaved = new EventAfterDetailSaved();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<EventAfterDetailSaved>()).Returns(eventAfterDetailSaved);

            _customerDetailViewModel = new CustomerDetailViewModel(_customerRepositoryMock.Object,
                                                                   _eventAggregatorMock.Object,
                                                                   _messageDialogServiceMock.Object,
                                                                   _accountManagerLookupDataServiceMock.Object);

            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);
            //Mockup list contains 3 entries + 1 NullLookupItem = 4 items:
            Assert.Equal(4, _customerDetailViewModel.LookupItemsAccountManager.Count());

            //Act
            //Change Mockup list to contain 4 entries isntead of 3 before:
            _lookupItemAccountManagerList = new List<LookupItem>
                {
                    new LookupItem {Id=1, DisplayMember = "Carl Manager" },
                    new LookupItem {Id=2, DisplayMember = "Fritz Manager" },
                    new LookupItem {Id=3, DisplayMember = "Fred Manager" },
                    new LookupItem {Id=4, DisplayMember = "Any OtherManager" }
                };
            var expectedNumberOfAccountManagersAfterAct = _lookupItemAccountManagerList.Count() + 1; // e Entries + 1 NullLookupItem

            _accountManagerLookupDataServiceMock.Setup(dp => dp.GetEntityLookupAsync()).ReturnsAsync(_lookupItemAccountManagerList);
            
            //Simulate an EventafterDetailSaved from an AccountManagerDetailViewModel:
            eventAfterDetailSaved.Publish(new EventAfterDetailSavedArgs
            {
                Id = 4,
                ViewModelName = nameof(AccountManagerDetailViewModel)
            });

            //Assert
            //ListLookupAccountManager should become 5 = 4 entries + 1 NullLookupItem
            Assert.Equal(expectedNumberOfAccountManagersAfterAct, _customerDetailViewModel.LookupItemsAccountManager.Count());
        }

        [Fact]
        public async void TabDisplayName_When_Customer_Is_Loaded_Should_Return_Customer_Name()
        {
            // Arrange
            var anyInTheMockDefinedCustomerId = 2;

            //Act
            await _customerDetailViewModel.LoadAsync(anyInTheMockDefinedCustomerId);


            //Assert
            Assert.Equal(_customerDetailViewModel.Customer.Name, _customerDetailViewModel.TabDisplayName);
        }

        [Fact]
        public async void ObservableCollectionWithNullLookupItemAndLookupItems_When_Called_With_3_Items_Should_Return_Collection_With_4_Items()
        {
            //Arrange
            var lookupItems = new List<LookupItem>
                {
                    new LookupItem {Id=1, DisplayMember = "Carl Manager" },
                    new LookupItem {Id=2, DisplayMember = "Carli Manager" },
                    new LookupItem {Id=3, DisplayMember = "Carls Manager" }
                };
            Func<Task<IEnumerable<LookupItem>>> funcGetLookupItemsAsync;
            funcGetLookupItemsAsync = async () => await Task.FromResult(lookupItems);
            var anzahlLookupItemsPlusNullLookupItem = lookupItems.Count + 1;

            //Act
            var result = await _customerDetailViewModel.ObservableCollectionWithNullLookupItemAndLookupItems(funcGetLookupItemsAsync);

            //Assert
            Assert.Equal(anzahlLookupItemsPlusNullLookupItem, result.Count);
            Assert.Contains(result, r => r.DisplayMember == "-");
            Assert.Contains(result, r => r.DisplayMember == "Carl Manager");
            Assert.Contains(result, r => r.DisplayMember == "Carli Manager");
            Assert.Contains(result, r => r.DisplayMember == "Carls Manager");
        }

        [Fact]
        public async void ObservableCollectionWithNullLookupItemAndLookupItems_When_Called_With_Null_Items_Should_Return_Collection_With_NullLookupItem()
        {
            //Arrange
            var lookupItems = new List<LookupItem>();
            Func<Task<IEnumerable<LookupItem>>> funcGetLookupItemsAsync;
            funcGetLookupItemsAsync = async () => await Task.FromResult(lookupItems);
            var anzahlLookupItemsPlusNullLookupItem = lookupItems.Count + 1;

            //Act
            var result = await _customerDetailViewModel.ObservableCollectionWithNullLookupItemAndLookupItems(funcGetLookupItemsAsync);

            //Assert
            Assert.Equal(anzahlLookupItemsPlusNullLookupItem, result.Count);
            Assert.Contains(result, r => r.DisplayMember == "-");
        }

        [Fact]
        public async void ObservableCollectionWithNullLookupItemAndLookupItems_When_Called_With_Null_Should_Throw_NullReferenceException()
        {
            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(
                  async () => await _customerDetailViewModel.ObservableCollectionWithNullLookupItemAndLookupItems(null));
        }
    }

}
