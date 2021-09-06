namespace Fanex.NAP.SPU.VR.Tests.Features.BetLimit
{
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using FluentAssertions;
    using NSubstitute;    
    using System.Collections.Generic;    
    using Xunit;
    
    public class BetLimitServiceTest
    {
        private readonly IBetLimitDataAccess dataAccess;
        private readonly IBetLimitService service;

        public BetLimitServiceTest()
        {
            dataAccess = Substitute.For<IBetLimitDataAccess>();

            service = new BetLimitService(dataAccess);
        }

        [Fact]
        public void GetBetLimitCustomers_WhenSuccess_ReturnCustomerList()
        {
            var expectedResult = BetLimitExpectedData.ExpectedCustomers;

            dataAccess.GetCustomersBetLimit().Returns(expectedResult);

            var result = service.GetBetLimitCustomers();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [MemberData(nameof(BetLimitExpectedData.ExpectedRemoveBetLimit), MemberType = typeof(BetLimitExpectedData))]
        public void RemoveBetLimit_WhenSuccess(IEnumerable<int> customers, int punterChunkSize)
        {
            service.UpdateBetLimit(customers, punterChunkSize);
        }

        [Theory]
        [MemberData(nameof(BetLimitExpectedData.ExpectedRemoveBetLimit), MemberType = typeof(BetLimitExpectedData))]
        public void UpdateBetLimit_WhenSuccess(IEnumerable<int> customers, int punterChunkSize)
        {
            service.UpdateBetLimit(customers, punterChunkSize);
        }

        [Theory]
        [InlineData("123, 456")]
        [InlineData("123, 789")]
        public void VerifyBetLimitCustomers_WhenSuccess(string customerList)
        {
            var (expectedCusts1, expectedCusts2) = BetLimitExpectedData.ExpectedVerifyCustomers;

            dataAccess.VerifyCustomersBetLimit(customerList).Returns((expectedCusts1, expectedCusts2));

            var result =  service.VerifyBetLimitCustomers(customerList);

            result.Should().BeEquivalentTo((expectedCusts1, expectedCusts2));
        }

        
    }
}
