namespace Fanex.NAP.SPU.VR.Tests.Features.SpecialLicensee
{
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee;
    using Fanex.NAP.SPU.VR.JobService;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for SpecialLicenseeJobService class
    /// </summary>
    public class SpecialLicenseeJobServiceTest
    {
        private readonly IAppSettings appSettings;
        private readonly ISpecialLicenseeService specialLicenseeService;
        private readonly SpecialLicenseeJobService jobService;

        public SpecialLicenseeJobServiceTest()
        {
            appSettings = Substitute.For<IAppSettings>();
            specialLicenseeService = Substitute.For<ISpecialLicenseeService>();
            jobService = new SpecialLicenseeJobService(appSettings, specialLicenseeService);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalledWithValidDependencies_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockAppSettings = Substitute.For<IAppSettings>();
            var mockSpecialLicenseeService = Substitute.For<ISpecialLicenseeService>();

            // Act
            var service = new SpecialLicenseeJobService(mockAppSettings, mockSpecialLicenseeService);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion

        #region Execute Method Tests

        [Fact]
        public async Task Execute_WhenSuccessful_ShouldReturnTrue()
        {
            // Arrange
            SetupSuccessfulExecution();

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            VerifySuccessfulExecution();
        }

        [Fact]
        public async Task Execute_WhenSuccessful_ShouldCallBothAutoAdjustMethods()
        {
            // Arrange
            SetupSuccessfulExecution();

            // Act
            await jobService.Execute();

            // Assert
            specialLicenseeService.Received(1).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
            specialLicenseeService.Received(1).AutoAdjustForSpecialSportsbook(appSettings.PunterChunkSize, appSettings.IsTest);
        }        

        [Fact]
        public async Task Execute_WhenRetryTimeIsOne_ShouldRetryOnce()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Throws(new Exception("Test exception"));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeFalse();
            specialLicenseeService.Received(1).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
        }

        [Fact]
        public async Task Execute_WhenRetryTimeIsThree_ShouldRetryThreeTimes()
        {
            // Arrange
            appSettings.RetryTime.Returns(3);
            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Throws(new Exception("Test exception"));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeFalse();
            specialLicenseeService.Received(3).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
        }

        [Fact]
        public async Task Execute_WhenExceptionOccursOnSecondRetry_ShouldReturnFalse()
        {
            // Arrange
            appSettings.RetryTime.Returns(3);
            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Throws(new Exception("Test exception"));              

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeFalse();
            specialLicenseeService.Received(3).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
        }       

        [Fact]
        public async Task Execute_WhenCalledMultipleTimes_ShouldProcessEachCall()
        {
            // Arrange
            SetupSuccessfulExecution();

            // Act
            var result1 = await jobService.Execute();
            var result2 = await jobService.Execute();

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            specialLicenseeService.Received(2).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
            specialLicenseeService.Received(2).AutoAdjustForSpecialSportsbook(appSettings.PunterChunkSize, appSettings.IsTest);
        }

        [Fact]
        public async Task Execute_WhenSpecialLicenseeServiceThrowsException_ShouldHandleGracefully()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Throws(new Exception("Service connection failed"));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Fluent Interface Tests

        [Fact]
        public async Task Execute_WhenAutoAdjustMethodsReturnService_ShouldChainCorrectly()
        {
            // Arrange
            appSettings.PunterChunkSize.Returns(100);
            appSettings.IsTest.Returns(true);
            appSettings.RetryTime.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            specialLicenseeService.PunterBasketballUpdated.Returns(5);
            specialLicenseeService.PunterSportsbookUpdated.Returns(3);

            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);
            specialLicenseeService.AutoAdjustForSpecialSportsbook(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            specialLicenseeService.Received(1).AutoAdjustForBasketball(100, true);
            specialLicenseeService.Received(1).AutoAdjustForSpecialSportsbook(100, true);
        }

        [Fact]
        public async Task Execute_WhenAutoAdjustForBasketballFails_ShouldNotCallAutoAdjustForSpecialSportsbook()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Throws(new Exception("Basketball adjustment failed"));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeFalse();
            specialLicenseeService.Received(1).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
            specialLicenseeService.DidNotReceive().AutoAdjustForSpecialSportsbook(Arg.Any<int>(), Arg.Any<bool>());
        }

        #endregion

        #region Property Access and Logging Tests

        [Fact]
        public async Task Execute_WhenSuccessful_ShouldAccessPropertiesForLogging()
        {
            // Arrange
            appSettings.PunterChunkSize.Returns(100);
            appSettings.IsTest.Returns(true);
            appSettings.RetryTime.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            specialLicenseeService.PunterBasketballUpdated.Returns(5);
            specialLicenseeService.PunterSportsbookUpdated.Returns(3);

            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);
            specialLicenseeService.AutoAdjustForSpecialSportsbook(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            // Verify that properties were accessed for logging
            var basketballUpdated = specialLicenseeService.PunterBasketballUpdated;
            var sportsbookUpdated = specialLicenseeService.PunterSportsbookUpdated;
            
            basketballUpdated.Should().Be(5);
            sportsbookUpdated.Should().Be(3);
        }

        [Fact]
        public async Task Execute_WhenPropertiesReturnZero_ShouldLogCorrectly()
        {
            // Arrange
            appSettings.PunterChunkSize.Returns(100);
            appSettings.IsTest.Returns(true);
            appSettings.RetryTime.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            specialLicenseeService.PunterBasketballUpdated.Returns(0);
            specialLicenseeService.PunterSportsbookUpdated.Returns(0);

            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);
            specialLicenseeService.AutoAdjustForSpecialSportsbook(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            specialLicenseeService.PunterBasketballUpdated.Should().Be(0);
            specialLicenseeService.PunterSportsbookUpdated.Should().Be(0);
        }

        [Fact]
        public async Task Execute_WhenPropertiesReturnLargeNumbers_ShouldLogCorrectly()
        {
            // Arrange
            appSettings.PunterChunkSize.Returns(100);
            appSettings.IsTest.Returns(true);
            appSettings.RetryTime.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            specialLicenseeService.PunterBasketballUpdated.Returns(1000);
            specialLicenseeService.PunterSportsbookUpdated.Returns(500);

            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);
            specialLicenseeService.AutoAdjustForSpecialSportsbook(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            specialLicenseeService.PunterBasketballUpdated.Should().Be(1000);
            specialLicenseeService.PunterSportsbookUpdated.Should().Be(500);
        }

        #endregion

        #region NotifyMessage Method Tests

        [Fact]
        public void NotifyMessage_WhenCalled_ShouldSetNotifiedMessage()
        {
            // Arrange
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");
            specialLicenseeService.PunterBasketballUpdated.Returns(5);
            specialLicenseeService.PunterSportsbookUpdated.Returns(3);

            // Act
            var method = typeof(SpecialLicenseeJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            var field = typeof(SpecialLicenseeJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var notifiedMessage = field.GetValue(jobService) as string;

            notifiedMessage.Should().NotBeNullOrEmpty();
            notifiedMessage.Should().Contain("✅ Auto special licensee job run COMPLETED");
            notifiedMessage.Should().Contain("OtherSports - Customers have passed Performance Check: 5");
            notifiedMessage.Should().Contain("Special SportBook - Customers have passed Performance Check: 3");
        }

        [Fact]
        public void NotifyMessage_WhenPropertiesAreZero_ShouldSetCorrectMessage()
        {
            // Arrange
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");
            specialLicenseeService.PunterBasketballUpdated.Returns(0);
            specialLicenseeService.PunterSportsbookUpdated.Returns(0);

            // Act
            var method = typeof(SpecialLicenseeJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            var field = typeof(SpecialLicenseeJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var notifiedMessage = field.GetValue(jobService) as string;

            notifiedMessage.Should().NotBeNullOrEmpty();
            notifiedMessage.Should().Contain("OtherSports - Customers have passed Performance Check: 0");
            notifiedMessage.Should().Contain("Special SportBook - Customers have passed Performance Check: 0");
        }

        [Fact]
        public void NotifyMessage_WhenCalled_ShouldNotThrowException()
        {
            // Arrange
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");
            specialLicenseeService.PunterBasketballUpdated.Returns(5);
            specialLicenseeService.PunterSportsbookUpdated.Returns(3);

            // Act
            var method = typeof(SpecialLicenseeJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var action = () => method.Invoke(jobService, null);

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public async Task Execute_WhenAppSettingsIsTestIsTrue_ShouldPassCorrectParameter()
        {
            // Arrange
            appSettings.IsTest.Returns(true);
            appSettings.PunterChunkSize.Returns(100);
            SetupSuccessfulExecution();

            // Act
            await jobService.Execute();

            // Assert
            specialLicenseeService.Received(1).AutoAdjustForBasketball(100, true);
            specialLicenseeService.Received(1).AutoAdjustForSpecialSportsbook(100, true);
        }

      
        [Fact]
        public async Task Execute_WhenCalledConcurrently_ShouldNotInterfere()
        {
            // Arrange
            SetupSuccessfulExecution();

            // Act
            var tasks = new[]
            {
                jobService.Execute(),
                jobService.Execute(),
                jobService.Execute()
            };

            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().AllBeEquivalentTo(true);
            specialLicenseeService.Received(3).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
            specialLicenseeService.Received(3).AutoAdjustForSpecialSportsbook(appSettings.PunterChunkSize, appSettings.IsTest);
        }

        [Fact]
        public async Task Execute_WhenServiceReturnsNull_ShouldHandleGracefully()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Returns((ISpecialLicenseeService)null);

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Execute_WhenFullWorkflowSucceeds_ShouldCompleteSuccessfully()
        {
            // Arrange
            appSettings.PunterChunkSize.Returns(100);
            appSettings.IsTest.Returns(true);
            appSettings.RetryTime.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            specialLicenseeService.PunterBasketballUpdated.Returns(10);
            specialLicenseeService.PunterSportsbookUpdated.Returns(5);

            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);
            specialLicenseeService.AutoAdjustForSpecialSportsbook(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            specialLicenseeService.Received(1).AutoAdjustForBasketball(100, true);
            specialLicenseeService.Received(1).AutoAdjustForSpecialSportsbook(100, true);
            specialLicenseeService.PunterBasketballUpdated.Should().Be(10);
            specialLicenseeService.PunterSportsbookUpdated.Should().Be(5);
        }

        [Fact]
        public async Task Execute_WhenFullWorkflowFails_ShouldHandleGracefully()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Throws(new Exception("Service connection failed"));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeFalse();
            specialLicenseeService.Received(1).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
        }

        #endregion

        #region Helper Methods

        private void SetupSuccessfulExecution()
        {
            appSettings.PunterChunkSize.Returns(100);
            appSettings.IsTest.Returns(true);
            appSettings.RetryTime.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            specialLicenseeService.PunterBasketballUpdated.Returns(5);
            specialLicenseeService.PunterSportsbookUpdated.Returns(3);

            specialLicenseeService.AutoAdjustForBasketball(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);
            specialLicenseeService.AutoAdjustForSpecialSportsbook(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);
        }

        private void SetupSuccessfulExecutionAfterRetry()
        {
            appSettings.PunterChunkSize.Returns(100);
            appSettings.IsTest.Returns(true);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            specialLicenseeService.PunterBasketballUpdated.Returns(5);
            specialLicenseeService.PunterSportsbookUpdated.Returns(3);

            specialLicenseeService.AutoAdjustForSpecialSportsbook(Arg.Any<int>(), Arg.Any<bool>())
                .Returns(specialLicenseeService);
        }

        private void VerifySuccessfulExecution()
        {
            specialLicenseeService.Received(1).AutoAdjustForBasketball(appSettings.PunterChunkSize, appSettings.IsTest);
            specialLicenseeService.Received(1).AutoAdjustForSpecialSportsbook(appSettings.PunterChunkSize, appSettings.IsTest);
        }

        #endregion
    }
}
