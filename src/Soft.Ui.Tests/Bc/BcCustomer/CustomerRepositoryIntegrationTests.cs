﻿using Soft.DataAccess;
using Soft.Ui.Bc.BcCustomer;
using System.Linq;
using Xunit;

namespace Soft.Ui.Tests.Bc.BcCustomer
{
    /// <summary>
    /// Integration test for CustomerRepository
    /// WARNING ! Test depends on database Seed data !!!
    /// </summary>
    public class CustomerRepositoryIntegrationTests
    {
        [Fact]
        public async void HasChanges_When_Context_Is_Changed_Should_Return_True()
        {
            //Arrange
            SoftDbContext _context = new SoftDbContext();
            var _customerRepository = new CustomerRepository(_context);

            //Act
            var anyCustomerFromDatabase = _context.Customers.FirstOrDefault();
            Assert.NotNull(anyCustomerFromDatabase);
            var customer = await _customerRepository.FindByIdAsync(anyCustomerFromDatabase.Id);
            customer.Name = "New Name";

            //Assert
            Assert.True(_customerRepository.HasChanges());
        }
        [Fact]
        public async void HasChanges_When_Context_Is_Not_Changed_Should_Return_False()
        {
            //Arrange
            SoftDbContext _context = new SoftDbContext();
            var _customerRepository = new CustomerRepository(_context);

            //Act
            var anyCustomerFromDatabase = _context.Customers.FirstOrDefault();
            Assert.NotNull(anyCustomerFromDatabase);
            var customer = await _customerRepository.FindByIdAsync(anyCustomerFromDatabase.Id);
            var oldCustomerName = customer.Name;
            customer.Name = "New Name";
            customer.Name = oldCustomerName;

            //Assert
            Assert.False(_customerRepository.HasChanges());
        }
        /// <summary>
        /// WARNING ! Test expects that first Customer in the repository (database) has bank accounts !!!
        /// </summary>
        [Fact]
        public async void FindByIdAsync_When_Called_Should_Return_Customer_including_BankAccounts()
        {
            //Arrange
            SoftDbContext _context = new SoftDbContext();
            var _customerRepository = new CustomerRepository(_context);

            //Act
            var firstRepositoryCustomer = _context.Customers.FirstOrDefault();
            Assert.NotNull(firstRepositoryCustomer);
            var customer = await _customerRepository.FindByIdAsync(firstRepositoryCustomer.Id);

            //Assert
            Assert.NotEmpty(customer.BankAccounts);
        }
        /// <summary>
        /// WARNING ! Test expects that Customers with Id = 1.000.000 is not in the database !!!
        /// </summary>
        [Fact]
        public async void FindByIdAsync_When_Called_With_NoneExisting_CustomerId_ie_1Mio_Should_Return_Null()
        {
            //Arrange
            SoftDbContext _context = new SoftDbContext();
            var _customerRepository = new CustomerRepository(_context);

            //Act
            var customer = await _customerRepository.FindByIdAsync(1000000);

            //Assert
            Assert.Null(customer);
        }
    }
}
