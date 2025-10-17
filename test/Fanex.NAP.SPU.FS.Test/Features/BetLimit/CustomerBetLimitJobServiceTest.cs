namespace Fanex.NAP.SPU.VR.Tests.Features.BetLimit
{
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using Fanex.NAP.SPU.VR.JobService;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for CustomerBetLimitJobService class
    /// </summary>
    public class CustomerBetLimitJobServiceTest
    {
        private readonly IAppSettings appSettings;
        private readonly IBetLimitService betLimitService;
        private readonly ICustomerBetLimitJobService jobService;

        public CustomerBetLimitJobServiceTest()
        {
            appSettings = Substitute.For<IAppSettings>();
            betLimitService = Substitute.For<IBetLimitService>();
            jobService = new CustomerBetLimitJobService(appSettings, betLimitService);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalled_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockAppSettings = Substitute.For<IAppSettings>();
            var mockBetLimitService = Substitute.For<IBetLimitService>();

            // Act
            var service = new CustomerBetLimitJobService(mockAppSettings, mockBetLimitService);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion                           
       
    }
}
