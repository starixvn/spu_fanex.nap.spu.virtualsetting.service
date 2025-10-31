namespace Fanex.NAP.SPU.VR.Tests.Features.BetLimit
{
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using FluentAssertions;
    using NSubstitute;    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    
    /// <summary>
    /// Unit tests for BetLimitService class
    /// </summary>
    public class BetLimitServiceTest
    {
        private readonly IBetLimitDataAccess dataAccess;
        private readonly IBetLimitService service;

        public BetLimitServiceTest()
        {
            dataAccess = Substitute.For<IBetLimitDataAccess>();
            service = new BetLimitService(dataAccess);
        }

        #region GetBetLimitCustomers Tests

        [Fact]
        public void GetBetLimitCustomers_WhenCalled_ShouldReturnExpectedResult()
        {
            // Arrange
            var expectedResult = BetLimitExpectedData.ExpectedCustomers;
            dataAccess.GetCustomersBetLimit().Returns(expectedResult);

            // Act
            var result = service.GetBetLimitCustomers();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            dataAccess.Received(1).GetCustomersBetLimit();
        }

        [Fact]
        public void GetBetLimitCustomers_WhenCalled_ShouldSetRawCustomerProperty()
        {
            // Arrange
            var expectedCustomers = new List<int> { 1, 2, 3, 4, 5 };
            dataAccess.GetCustomersBetLimit().Returns(expectedCustomers);

            // Act
            service.GetBetLimitCustomers();

            // Assert
            service.RawCustomer.Should().Be(5);
        }

        [Fact]
        public void GetBetLimitCustomers_WhenDataAccessReturnsEmpty_ShouldSetRawCustomerToZero()
        {
            // Arrange
            var emptyCustomers = new List<int>();
            dataAccess.GetCustomersBetLimit().Returns(emptyCustomers);

            // Act
            service.GetBetLimitCustomers();

            // Assert
            service.RawCustomer.Should().Be(0);
        }

        #endregion

        #region RemoveBetLimit Tests

        [Theory]
        [MemberData(nameof(BetLimitExpectedData.ExpectedRemoveBetLimit), MemberType = typeof(BetLimitExpectedData))]
        public void RemoveBetLimit_WhenCalled_ShouldCallDataAccessWithCorrectParameters(IEnumerable<int> customers, int punterChunkSize)
        {
            // Act
            service.RemoveBetLimit(customers, punterChunkSize);

            // Assert
            dataAccess.Received().RemoveBetLimit(Arg.Any<string>());
            service.RemovedCustomers.Should().Be(customers.Count());
        }

        [Fact]
        public void RemoveBetLimit_WhenEmptyCustomerList_ShouldNotCallDataAccess()
        {
            // Arrange
            var emptyCustomers = new List<int>();

            // Act
            service.RemoveBetLimit(emptyCustomers, 10);

            // Assert
            dataAccess.DidNotReceive().RemoveBetLimit(Arg.Any<string>());
            service.RemovedCustomers.Should().Be(0);
        }

        [Fact]
        public void RemoveBetLimit_WhenNullCustomerList_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<int> nullCustomers = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.RemoveBetLimit(nullCustomers, 10));
        }

        [Fact]
        public void RemoveBetLimit_WhenSingleChunk_ShouldCallDataAccessOnce()
        {
            // Arrange
            var customers = new List<int> { 1, 2, 3 };
            var chunkSize = 10;

            // Act
            service.RemoveBetLimit(customers, chunkSize);

            // Assert
            dataAccess.Received(1).RemoveBetLimit("1,2,3");
            service.RemovedCustomers.Should().Be(3);
        }

        [Fact]
        public void RemoveBetLimit_WhenMultipleChunks_ShouldCallDataAccessMultipleTimes()
        {
            // Arrange
            var customers = new List<int> { 1, 2, 3, 4, 5 };
            var chunkSize = 2;

            // Act
            service.RemoveBetLimit(customers, chunkSize);

            // Assert
            dataAccess.Received(3).RemoveBetLimit(Arg.Any<string>());
            dataAccess.Received().RemoveBetLimit("1,2");
            dataAccess.Received().RemoveBetLimit("3,4");
            dataAccess.Received().RemoveBetLimit("5");
            service.RemovedCustomers.Should().Be(5);
        }

        #endregion

        #region UpdateBetLimit Tests

        [Theory]
        [MemberData(nameof(BetLimitExpectedData.ExpectedRemoveBetLimit), MemberType = typeof(BetLimitExpectedData))]
        public void UpdateBetLimit_WhenCalled_ShouldCallDataAccessWithCorrectParameters(IEnumerable<int> customers, int punterChunkSize)
        {
            // Act
            service.UpdateBetLimit(customers, punterChunkSize);

            // Assert
            dataAccess.Received().UpdateBetLimit(Arg.Any<string>());
            service.UpdateCustomers.Should().Be(customers.Count());
        }

        [Fact]
        public void UpdateBetLimit_WhenEmptyCustomerList_ShouldNotCallDataAccess()
        {
            // Arrange
            var emptyCustomers = new List<int>();

            // Act
            service.UpdateBetLimit(emptyCustomers, 10);

            // Assert
            dataAccess.DidNotReceive().UpdateBetLimit(Arg.Any<string>());
            service.UpdateCustomers.Should().Be(0);
            service.PuntersUpdated.Should().Be(0);
        }

        [Fact]
        public void UpdateBetLimit_WhenNullCustomerList_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEnumerable<int> nullCustomers = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.UpdateBetLimit(nullCustomers, 10));
        }

        [Fact]
        public void UpdateBetLimit_WhenSingleChunk_ShouldCallDataAccessOnceAndUpdatePuntersUpdated()
        {
            // Arrange
            var customers = new List<int> { 1, 2, 3 };
            var chunkSize = 10;

            // Act
            service.UpdateBetLimit(customers, chunkSize);

            // Assert
            dataAccess.Received(1).UpdateBetLimit("1,2,3");
            service.UpdateCustomers.Should().Be(3);
            service.PuntersUpdated.Should().Be(3);
        }

        [Fact]
        public void UpdateBetLimit_WhenMultipleChunks_ShouldCallDataAccessMultipleTimesAndAccumulatePuntersUpdated()
        {
            // Arrange
            var customers = new List<int> { 1, 2, 3, 4, 5 };
            var chunkSize = 2;

            // Act
            service.UpdateBetLimit(customers, chunkSize);

            // Assert
            dataAccess.Received(3).UpdateBetLimit(Arg.Any<string>());
            dataAccess.Received().UpdateBetLimit("1,2");
            dataAccess.Received().UpdateBetLimit("3,4");
            dataAccess.Received().UpdateBetLimit("5");
            service.UpdateCustomers.Should().Be(5);
            service.PuntersUpdated.Should().Be(5);
        }

        [Fact]
        public void UpdateBetLimit_WhenCalledMultipleTimes_ShouldAccumulatePuntersUpdated()
        {
            // Arrange
            var customers1 = new List<int> { 1, 2 };
            var customers2 = new List<int> { 3, 4, 5 };

            // Act
            service.UpdateBetLimit(customers1, 10);
            service.UpdateBetLimit(customers2, 10);

            // Assert
            service.PuntersUpdated.Should().Be(5);
            dataAccess.Received(2).UpdateBetLimit(Arg.Any<string>());
        }

        #endregion

        #region VerifyBetLimitCustomers Tests

        [Theory]
        [InlineData("123, 456")]
        [InlineData("123, 789")]
        [InlineData("single_customer")]
        public void VerifyBetLimitCustomers_WhenCalled_ShouldReturnExpectedResult(string customerList)
        {
            // Arrange
            var (expectedCusts1, expectedCusts2) = BetLimitExpectedData.ExpectedVerifyCustomers;
            dataAccess.VerifyCustomersBetLimit(customerList).Returns((expectedCusts1, expectedCusts2));

            // Act
            var result = service.VerifyBetLimitCustomers(customerList);

            // Assert
            result.Should().BeEquivalentTo((expectedCusts1, expectedCusts2));
            dataAccess.Received(1).VerifyCustomersBetLimit(customerList);
        }

        [Fact]
        public void VerifyBetLimitCustomers_WhenDataAccessReturnsEmptyTuples_ShouldReturnEmptyCollections()
        {
            // Arrange
            var emptyResult = (new List<int>(), new List<int>());
            dataAccess.VerifyCustomersBetLimit(Arg.Any<string>()).Returns(emptyResult);

            // Act
            var result = service.VerifyBetLimitCustomers("test");

            // Assert
            result.Item1.Should().BeEmpty();
            result.Item2.Should().BeEmpty();
            dataAccess.Received(1).VerifyCustomersBetLimit("test");
        }

        [Fact]
        public void VerifyBetLimitCustomers_WhenDataAccessReturnsPartialData_ShouldReturnCorrectData()
        {
            // Arrange
            var custs1 = new List<int> { 123 };
            var custs2 = new List<int>();
            var expectedResult = (custs1, custs2);
            dataAccess.VerifyCustomersBetLimit(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = service.VerifyBetLimitCustomers("test");

            // Assert
            result.Item1.Should().HaveCount(1);
            result.Item1.Should().BeEquivalentTo(custs1);
            result.Item2.Should().BeEmpty();
            dataAccess.Received(1).VerifyCustomersBetLimit("test");
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Properties_WhenServiceInitialized_ShouldHaveDefaultValues()
        {
            // Act & Assert
            service.PuntersUpdated.Should().Be(0);
            service.RemovedCustomers.Should().Be(0);
            service.UpdateCustomers.Should().Be(0);
            service.RawCustomer.Should().Be(0);
        }

        #endregion
    }
}