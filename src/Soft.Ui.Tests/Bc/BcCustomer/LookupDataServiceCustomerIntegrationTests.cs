using Soft.DataAccess;
using Soft.DataAccess.Shared.Lookup;
using Soft.Model;
using System;
using System.Linq;
using Xunit;

namespace Soft.Ui.Tests.Bc.BcCustomer
{
    public class LookupDataServiceCustomerIntegrationTests
    {
        const string cAnyString = "Any Display Member Value";

        private class MockLookupDataServiceCustomer : GenericLookupDataService<Customer>
        {
            public MockLookupDataServiceCustomer(Func<SoftDbContext> dbContextCreator)
            : base(dbContextCreator)
            {
            }
            public override string LookupDisplayMemberValue(Customer customer)
            {
                return cAnyString;
            }
        }

        /// <summary>
        /// WARNING ! Test expects that three Customer in the repository (database) !!!
        /// </summary>
        [Fact]
        public async void GetEntityLookupAsync_Returns_LookupItems()
        {
            //Arrange
            SoftDbContext _context = new SoftDbContext();
            var mockLookupDataServiceCustomer = new MockLookupDataServiceCustomer(() => _context);

            //Act
            var result = await mockLookupDataServiceCustomer.GetEntityLookupAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(cAnyString, result.FirstOrDefault().DisplayMember);
        }
    }
}
