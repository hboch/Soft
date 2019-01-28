using Soft.Model;
using Soft.Ui.Bc.BcCustomer;
using Soft.Ui.Tests.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Soft.Ui.Tests.Bc.BcCustomer
{
    public class CustomerWrapperTests
    {
        private Customer _anyCustomer;
        private CustomerWrapper _customerWrapper;
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
        private class BankAccountTest : BankAccount
        {
            public BankAccountTest(int id)
            {
                Id = id;
            }
        }

        public CustomerWrapperTests()
        {
            //Arrange
            var accountManager = new AccountManager();
            var bankAccounts = new List<BankAccount>()
                {
                    new BankAccountTest(id:1) {AccountNo = "1234567890" },
                    new BankAccountTest(id:2) {AccountNo = "0000000001" }
            };
            _anyCustomer = new CustomerTest(id:2)
            {                
                Name = "AnySurname AnyLastName",
                InternetAdress = "http://microsoft.com",
                AccountManager = accountManager,
                BankAccounts = bankAccounts
            };

            _customerWrapper = new CustomerWrapper(_anyCustomer);

        }
        #region KonstruktorTests
        [Fact]
        public void CustomerWrapper_When_Instantiated_Should_Contain_Model_In_Model_Property()
        {
            //Assert
            Assert.Equal(_anyCustomer, _customerWrapper.Entity);
        }

        [Fact]
        public void CustomerWrapper_When_Instantiated_Should_Property_Name_Contain_Model_Property_Name_Value()
        {
            //Assert
            Assert.Equal(_anyCustomer.Name, _customerWrapper.Entity.Name);
        }

        [Fact]
        public void CustomerWrapper_When_Instantiated_With_Null_Customer_Should_Throw_ArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new CustomerWrapper(null));
        }

        //[Fact]
        //public void CustomerWrapper_When_Instantiated_Should_Initialize_AccountManager_Property()
        //{
        //    //Assert
        //    Assert.NotNull(_customerWrapper.AccountManager);
        //    Assert.Equal(_anyCustomer.AccountManager, _customerWrapper.Model.AccountManager);
        //}
        //[Fact]
        //public void CustomerWrapper_When_Instantiated_With_Customer_With_Null_AccountManager_Should_Throw_ArgumentException()
        //{
        //    //Arrange
        //    _anyCustomer.AccountManager = null;

        //    //Act & Assert
        //    Assert.Throws<ArgumentException>(() => new CustomerWrapper(_anyCustomer));
        //}
        //[Fact]
        //public void CustomerWrapper_When_Instantiated_With_Customer_With_Null_Accounts_Should_Throw_ArgumentException()
        //{
        //    //Arrange
        //    _anyCustomer.Accounts = null;

        //    //Act & Assert
        //    Assert.Throws<ArgumentException>(() => new CustomerWrapper(_anyCustomer));
        //}
        //[Fact]
        //public void CustomerWrapper_When_Instantiated_Should_Initialize_Accounts_Property()
        //{
        //    //Assert
        //    Assert.NotNull(_customerWrapper.Accounts);
        //    Assert.Equal(_anyCustomer.Accounts.Count, _customerWrapper.Accounts.Count);
        //    Assert.True(_anyCustomer.Accounts.All
        //        (customerKonto => _customerWrapper.Accounts.Any(wrapperKonto => wrapperKonto == customerKonto)));
        //}
        #endregion

        #region PropertyChangedTests
        [Fact]
        public void Name_When_Set_Should_Raise_PropertyChangedEvent()
        {
            //Act
            var fired = _customerWrapper.IsPropertyChangedFired(() => { _customerWrapper.Name = "Changed"; },
            nameof(_customerWrapper.Name));

            //Assert
            Assert.True(fired);
        }
        [Fact]
        public void CustomerSince_When_Set_Should_Raise_PropertyChangedEvent()
        {
            //Act
            var fired = _customerWrapper.IsPropertyChangedFired(() =>
            { _customerWrapper.CustomerSince = Convert.ToDateTime("31.12.2018"); },
            nameof(_customerWrapper.CustomerSince));

            //Assert
            Assert.True(fired);
        }
        [Fact]
        public void InternetAdress_When_Set_Should_Raise_PropertyChangedEvent()
        {
            //Act
            var fired = _customerWrapper.IsPropertyChangedFired
                (
                    () =>
                        { _customerWrapper.InternetAdress = "http://google.com"; },
                        nameof(_customerWrapper.InternetAdress)
                );

            //Assert
            Assert.True(fired);
        }
        #endregion

        #region ValidationTests        
        [Fact]
        public void Name_When_Set_With_Valid_Name_Should_Result_In_Not_HasErrors()
        {
            //Act
            _customerWrapper.Name = "NewSurname NewLastname";

            //Assert
            Assert.True(!_customerWrapper.HasErrors);
        }

        [Fact]
        public void Name_When_Set_With_Valid_Name_Should_Result_In_No_Errors()
        {
            //Act
            _customerWrapper.Name = "Surname Lastname";

            //Assert
            var errors = _customerWrapper.GetErrors(nameof(_customerWrapper.Name));
            Assert.Null(errors);
        }

        [Fact]
        public void Name_When_Set_With_Less_Than_2_Characters_Should_Result_In_HasErrors()
        {
            //Act
            _customerWrapper.Name = ".";

            //Assert
            Assert.True(_customerWrapper.HasErrors);
        }

        [Fact]
        public void Name_With_Max_50_Chars_Data_Annotation_When_Set_With_More_Than_50_Chars_Should_Result_In_One_Error()
        {
            //Act
            _customerWrapper.Name = "012345678901234567890123456789 012345678901234567890123456789";

            //Assert
            var errors = _customerWrapper.GetErrors(nameof(_customerWrapper.Name));
            Assert.Single(_customerWrapper.GetErrors(nameof(_customerWrapper.Name)));
        }

        [Fact]
        public void Name_When_Set_With_Less_Than_2_Characters_And_Without_Space_Should_Result_In_Two_Errors()
        {
            //Act
            _customerWrapper.Name = ".";

            //Assert
            var errors = _customerWrapper.GetErrors(nameof(_customerWrapper.Name));
            Assert.Equal(2, ((List<string>)_customerWrapper.GetErrors(nameof(_customerWrapper.Name))).Count);

        }

        [Fact]
        public void Name_When_Set_Twice_With_Less_Than_2_Chars_And_Without_Space_Should_Result_In_Two_Errors()
        {
            //Act
            _customerWrapper.Name = ".";
            _customerWrapper.Name = "1";

            //Assert
            Assert.Equal(2, ((List<string>)_customerWrapper.GetErrors(nameof(_customerWrapper.Name))).Count);
        }

        [Fact]
        public void Name_When_Set_Twice_With_Less_Than_2_Chars_And_Then_With_A_Valid_Name_Should_Result_In_No_Error()
        {
            //Act
            _customerWrapper.Name = ".";
            _customerWrapper.Name = "Surname Lastname";

            //Assert
            Assert.Null(_customerWrapper.GetErrors(nameof(_customerWrapper.Name)));
        }

        [Fact]
        public void Name_When_Set_With_Less_Than_2_Chars_And_Valid_And_Then_With_More_Than_50_Chars_Should_Result_In_One_Error()
        {

            //Act
            _customerWrapper.Name = ".";
            _customerWrapper.Name = "NewSurname NewLastname";
            _customerWrapper.Name = "012345678901234567890123456789 012345678901234567890123456789";

            //Assert
            Assert.Single(_customerWrapper.GetErrors(nameof(_customerWrapper.Name)));
        }

        [Fact]
        public void Name_When_Set_Valid_And_Then_Invalid_And_Then_Valid_Should_Result_In_No_Errors()
        {
            //Act
            _customerWrapper.Name = "NewSurname NewLastname";
            _customerWrapper.Name = "SurnameLastname";
            _customerWrapper.Name = "NewSurname NewLastname";

            //Assert
            Assert.Null(_customerWrapper.GetErrors(nameof(_customerWrapper.Name)));
        }
                
        [Fact]
        public void Name_When_Set_With_Less_Than_2_Characters_Should_Raise_ErrorsChangedEvent()
        {
            var fired = false;
            _customerWrapper.ErrorsChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_customerWrapper.Name))
                {
                    fired = true;
                }
            };

            //Act
            _customerWrapper.Name = ".";

            //Assert
            Assert.True(fired);
        }

        [Fact]
        public void Name_When_Set_With_Valid_Name_Should_Not_Raise_ErrorsChangedEvent()
        {
            var fired = false;

            _customerWrapper.ErrorsChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_customerWrapper.Name))
                {
                    fired = true;
                }
            };

            //Act
            _customerWrapper.Name = "NewSurname NewLastname";

            //Assert
            Assert.False(fired);
        }

        [Fact]
        public void Name_When_Set_With_Less_Than_2_Characters_Should_Raise_PropertyChanged_For_HasErrors()
        {
            //Act
            var fired = _customerWrapper.IsPropertyChangedFired(() => { _customerWrapper.Name = "."; }, nameof(_customerWrapper.HasErrors));

            //Assert
            Assert.True(fired);
        }

        [Fact]
        public void Name_When_Set_With_Valid_Name_Should_Not_Raise_PropertyChanged_For_HasErrors()
        {
            //Act
            var fired = _customerWrapper.IsPropertyChangedFired(() => { _customerWrapper.Name = "NewSurname NewLastname"; }, nameof(_customerWrapper.HasErrors));

            //Assert
            Assert.False(fired);
        }
        #endregion
    }
}
