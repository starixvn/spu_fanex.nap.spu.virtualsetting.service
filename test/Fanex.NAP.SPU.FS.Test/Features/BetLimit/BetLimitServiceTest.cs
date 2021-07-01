namespace Fanex.NAP.SPU.VR.Tests.Features.BetLimit
{
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
            var expectedResult = new List<int> { 123, 456, 789 };

            dataAccess.GetCustomersBetLimit().Returns(expectedResult);

            var result = service.GetBetLimitCustomers();

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
