using Soft.DataAccess;
using Soft.Ui.Bc.BcBroker;
using System.Linq;
using Xunit;

namespace Soft.Ui.Tests.Bc.BcBroker
{
    public class BrokerRepositoryIntegrationTests
    {
        /// <summary>
        /// WARNING ! Test expects that first Broker in the repository (database) has releated Customers !!!
        /// </summary>
        [Fact]
        public async void GetByIdAsync_When_Called_Should_Return_Broker_including_Customers()
        {
            //Arrange
            SoftDbContext _context = new SoftDbContext();
            var _brokerRepository = new BrokerRepository(_context);

            //Act
            var firstBrokerInRepository = _context.Brokers.FirstOrDefault();
            Assert.NotNull(firstBrokerInRepository);
            var broker = await _brokerRepository.FindByIdAsync(firstBrokerInRepository.Id);

            //Assert
            Assert.NotEmpty(broker.Customers);
        }
    }
}
