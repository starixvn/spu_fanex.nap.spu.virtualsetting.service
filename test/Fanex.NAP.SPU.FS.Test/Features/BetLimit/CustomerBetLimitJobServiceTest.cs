namespace Fanex.NAP.SPU.VR.Tests.Features.BetLimit
{
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using Fanex.NAP.SPU.VR.Features.Notification;
    using Fanex.NAP.SPU.VR.JobService;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// Unit tests for CustomerBetLimitJobService class
    /// </summary>
    public class CustomerBetLimitJobServiceTest
    {
        private readonly IAppSettings appSettings;
        private readonly IBetLimitService betLimitService;
        private readonly ICustomerBetLimitJobService jobService;
        private readonly INotificationService notificationService;

        public CustomerBetLimitJobServiceTest()
        {
            appSettings = Substitute.For<IAppSettings>();
            betLimitService = Substitute.For<IBetLimitService>();
            notificationService = Substitute.For<INotificationService>();

            jobService = new CustomerBetLimitJobService(appSettings, betLimitService, notificationService);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalled_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockAppSettings = Substitute.For<IAppSettings>();
            var mockBetLimitService = Substitute.For<IBetLimitService>();
            var notificationService = Substitute.For<INotificationService>();
         
            // Act
            var service = new CustomerBetLimitJobService(mockAppSettings, mockBetLimitService, notificationService);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion                           
       
    }
}
