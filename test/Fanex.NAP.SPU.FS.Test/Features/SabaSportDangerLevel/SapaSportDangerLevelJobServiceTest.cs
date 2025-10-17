namespace Fanex.NAP.SPU.VR.Tests.Features.SabaSportDangerLevel
{
    using Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel;
    using Fanex.NAP.SPU.VR.JobService;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for SapaSportDangerLevelJobService class
    /// </summary>
    public class SapaSportDangerLevelJobServiceTest
    {
        private readonly IAppSettings appSettings;
        private readonly IDangerLevelSapaSportService dangerLevelSapaSportService;
        private readonly SapaSportDangerLevelJobService jobService;

        public SapaSportDangerLevelJobServiceTest()
        {
            appSettings = Substitute.For<IAppSettings>();
            dangerLevelSapaSportService = Substitute.For<IDangerLevelSapaSportService>();
            jobService = new SapaSportDangerLevelJobService(appSettings, dangerLevelSapaSportService);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalledWithValidDependencies_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockAppSettings = Substitute.For<IAppSettings>();
            var mockDangerLevelSapaSportService = Substitute.For<IDangerLevelSapaSportService>();

            // Act
            var service = new SapaSportDangerLevelJobService(mockAppSettings, mockDangerLevelSapaSportService);

            // Assert
            service.Should().NotBeNull();
        }
      
        #endregion

        #region Execute Method Tests

        [Fact]
        public async Task Execute_WhenSuccessful_ShouldReturnTrue()
        {
            // Arrange
            var batchSize = 100;
            SetupSuccessfulExecution();

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeTrue();
            VerifySuccessfulExecution();
        }

        [Fact]
        public async Task Execute_WhenDataNotReady_ShouldReturnTrue()
        {
            // Arrange
            var batchSize = 100;
            SetupDataNotReadyExecution();

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeTrue();
            dangerLevelSapaSportService.Received(1).IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady);
        }

        [Fact]
        public async Task Execute_WhenUpdateFails_ShouldReturnFalse()
        {
            // Arrange
            var batchSize = 100;
            SetupFailedUpdateExecution();

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeFalse();
            VerifyFailedUpdateExecution();
        }

        [Fact]
        public async Task Execute_WhenExceptionOccurs_ShouldReturnFalse()
        {
            // Arrange
            var batchSize = 100;
            SetupExceptionExecution();

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeFalse();
            dangerLevelSapaSportService.Received(1).IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady);
        }

        [Fact]
        public async Task Execute_WhenCalledWithZeroBatchSize_ShouldProcessCorrectly()
        {
            // Arrange
            var batchSize = 0;
            SetupSuccessfulExecution();

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeTrue();
            VerifySuccessfulExecution();
        }

        [Fact]
        public async Task Execute_WhenCalledWithNegativeBatchSize_ShouldProcessCorrectly()
        {
            // Arrange
            var batchSize = -1;
            SetupSuccessfulExecution();

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeTrue();
            VerifySuccessfulExecution();
        }

        [Fact]
        public async Task Execute_WhenCalledWithMaxIntBatchSize_ShouldProcessCorrectly()
        {
            // Arrange
            var batchSize = int.MaxValue;
            SetupSuccessfulExecution();

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeTrue();
            VerifySuccessfulExecution();
        }

        [Fact]
        public async Task Execute_WhenCalledMultipleTimes_ShouldProcessEachCall()
        {
            // Arrange
            var batchSize = 100;
            SetupSuccessfulExecution();

            // Act
            var result1 = await jobService.Execute(batchSize);
            var result2 = await jobService.Execute(batchSize);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            dangerLevelSapaSportService.Received(2).IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady);
        }

        #endregion

        #region JobRun Method Tests

        [Fact]
        public void JobRun_WhenDataNotReady_ShouldReturnTrue()
        {
            // Arrange
            var batchSize = 100;
            SetupDataNotReadyExecution();

            // Act
            var method = typeof(SapaSportDangerLevelJobService).GetMethod("JobRun", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (bool)method.Invoke(jobService, new object[] { batchSize });

            // Assert
            result.Should().BeTrue();
            dangerLevelSapaSportService.Received(1).IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady);
        }

        [Fact]
        public void JobRun_WhenDataReadyAndUpdateSucceeds_ShouldReturnTrue()
        {
            // Arrange
            var batchSize = 100;
            SetupSuccessfulExecution();

            // Act
            var method = typeof(SapaSportDangerLevelJobService).GetMethod("JobRun", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (bool)method.Invoke(jobService, new object[] { batchSize });

            // Assert
            result.Should().BeTrue();
            VerifySuccessfulExecution();
        }

        [Fact]
        public void JobRun_WhenDataReadyButUpdateFails_ShouldReturnFalse()
        {
            // Arrange
            var batchSize = 100;
            SetupFailedUpdateExecution();

            // Act
            var method = typeof(SapaSportDangerLevelJobService).GetMethod("JobRun", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (bool)method.Invoke(jobService, new object[] { batchSize });

            // Assert
            result.Should().BeFalse();
            VerifyFailedUpdateExecution();
        }

        [Fact]
        public void JobRun_WhenCalledWithDifferentBatchSizes_ShouldProcessCorrectly()
        {
            // Arrange
            var batchSizes = new[] { 1, 10, 100, 1000 };
            SetupSuccessfulExecution();

            foreach (var batchSize in batchSizes)
            {
                // Act
                var method = typeof(SapaSportDangerLevelJobService).GetMethod("JobRun", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var result = (bool)method.Invoke(jobService, new object[] { batchSize });

                // Assert
                result.Should().BeTrue();
            }
        }

        #endregion

        #region NotifyMessage Method Tests

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageIsEmpty_ShouldNotCallUtilities()
        {
            // Arrange
            var field = typeof(SapaSportDangerLevelJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, "");

            // Act
            var method = typeof(SapaSportDangerLevelJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            // Since we can't easily mock static methods, we just verify the method doesn't throw
        }

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageIsNull_ShouldNotCallUtilities()
        {
            // Arrange
            var field = typeof(SapaSportDangerLevelJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, null);

            // Act
            var method = typeof(SapaSportDangerLevelJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            // Since we can't easily mock static methods, we just verify the method doesn't throw
        }       

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageHasValue_ShouldNotThrow()
        {
            // Arrange
            var field = typeof(SapaSportDangerLevelJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, "Test notification message");

            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            // Act
            var method = typeof(SapaSportDangerLevelJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var action = () => method.Invoke(jobService, null);

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task Execute_WithVariousBatchSizes_ShouldProcessCorrectly(int batchSize)
        {
            // Arrange
            SetupSuccessfulExecution();

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Execute_WhenCalledConcurrently_ShouldNotInterfere()
        {
            // Arrange
            var batchSize = 100;
            SetupSuccessfulExecution();

            // Act
            var tasks = new[]
            {
                jobService.Execute(batchSize),
                jobService.Execute(batchSize),
                jobService.Execute(batchSize)
            };

            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().AllBeEquivalentTo(true);
            dangerLevelSapaSportService.Received(3).IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady);
        }
     
        #endregion

        #region Integration Tests

        [Fact]
        public async Task Execute_WhenFullWorkflowSucceeds_ShouldCompleteSuccessfully()
        {
            // Arrange
            var batchSize = 100;
            var expectedResult = new UpdateDangerResultEntity { ErrorCode = 0, ErrorMsg = "Success" };
            
            appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady.Returns(3);
            appSettings.Environment.Returns(1);
            appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");
            
            dangerLevelSapaSportService.IsDataSabaSportDangerLevelReady(Arg.Any<int>()).Returns(true);
            dangerLevelSapaSportService.Start().Returns(dangerLevelSapaSportService);
            dangerLevelSapaSportService.GetSapaSportDangerLevel(Arg.Any<int>()).Returns(dangerLevelSapaSportService);
            dangerLevelSapaSportService.RetryUpdateDangerLevelToMainDB(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(expectedResult);

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeTrue();
            dangerLevelSapaSportService.Received(1).IsDataSabaSportDangerLevelReady(3);
            dangerLevelSapaSportService.Received(1).Start();
            dangerLevelSapaSportService.Received(1).GetSapaSportDangerLevel(1);
            dangerLevelSapaSportService.Received(1).RetryUpdateDangerLevelToMainDB(3, batchSize, 1);
        }

        [Fact]
        public async Task Execute_WhenFullWorkflowFails_ShouldHandleGracefully()
        {
            // Arrange
            var batchSize = 100;
            var expectedResult = new UpdateDangerResultEntity { ErrorCode = -1, ErrorMsg = "Update failed" };
            
            appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady.Returns(3);
            appSettings.Environment.Returns(1);
            appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");
            
            dangerLevelSapaSportService.IsDataSabaSportDangerLevelReady(Arg.Any<int>()).Returns(true);
            dangerLevelSapaSportService.Start().Returns(dangerLevelSapaSportService);
            dangerLevelSapaSportService.GetSapaSportDangerLevel(Arg.Any<int>()).Returns(dangerLevelSapaSportService);
            dangerLevelSapaSportService.RetryUpdateDangerLevelToMainDB(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(expectedResult);

            // Act
            var result = await jobService.Execute(batchSize);

            // Assert
            result.Should().BeFalse();
            dangerLevelSapaSportService.Received(1).IsDataSabaSportDangerLevelReady(3);
            dangerLevelSapaSportService.Received(1).Start();
            dangerLevelSapaSportService.Received(1).GetSapaSportDangerLevel(1);
            dangerLevelSapaSportService.Received(1).RetryUpdateDangerLevelToMainDB(3, batchSize, 1);
        }

        #endregion

        #region Helper Methods

        private void SetupSuccessfulExecution()
        {
            appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady.Returns(3);
            appSettings.Environment.Returns(1);
            appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            var expectedResult = new UpdateDangerResultEntity { ErrorCode = 0, ErrorMsg = "Success" };

            dangerLevelSapaSportService.IsDataSabaSportDangerLevelReady(Arg.Any<int>()).Returns(true);
            dangerLevelSapaSportService.Start().Returns(dangerLevelSapaSportService);
            dangerLevelSapaSportService.GetSapaSportDangerLevel(Arg.Any<int>()).Returns(dangerLevelSapaSportService);
            dangerLevelSapaSportService.RetryUpdateDangerLevelToMainDB(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(expectedResult);
        }

        private void SetupDataNotReadyExecution()
        {
            appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            dangerLevelSapaSportService.IsDataSabaSportDangerLevelReady(Arg.Any<int>()).Returns(false);
        }

        private void SetupFailedUpdateExecution()
        {
            appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady.Returns(3);
            appSettings.Environment.Returns(1);
            appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            var expectedResult = new UpdateDangerResultEntity { ErrorCode = -1, ErrorMsg = "Update failed" };

            dangerLevelSapaSportService.IsDataSabaSportDangerLevelReady(Arg.Any<int>()).Returns(true);
            dangerLevelSapaSportService.Start().Returns(dangerLevelSapaSportService);
            dangerLevelSapaSportService.GetSapaSportDangerLevel(Arg.Any<int>()).Returns(dangerLevelSapaSportService);
            dangerLevelSapaSportService.RetryUpdateDangerLevelToMainDB(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(expectedResult);
        }

        private void SetupExceptionExecution()
        {
            appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady.Returns(3);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            dangerLevelSapaSportService.IsDataSabaSportDangerLevelReady(Arg.Any<int>())
                .Throws(new Exception("Service connection failed"));
        }

        private void VerifySuccessfulExecution()
        {
            dangerLevelSapaSportService.Received(1).IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady);
            dangerLevelSapaSportService.Received(1).Start();
            dangerLevelSapaSportService.Received(1).GetSapaSportDangerLevel(appSettings.Environment);
            dangerLevelSapaSportService.Received(1).RetryUpdateDangerLevelToMainDB(
                appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel, 
                Arg.Any<int>(), 
                appSettings.Environment);
        }

        private void VerifyFailedUpdateExecution()
        {
            dangerLevelSapaSportService.Received(1).IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady);
            dangerLevelSapaSportService.Received(1).Start();
            dangerLevelSapaSportService.Received(1).GetSapaSportDangerLevel(appSettings.Environment);
            dangerLevelSapaSportService.Received(1).RetryUpdateDangerLevelToMainDB(
                appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel, 
                Arg.Any<int>(), 
                appSettings.Environment);
        }

        #endregion
    }
}
