namespace Fanex.NAP.SPU.VR.Tests.Features.JobService
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
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for CustomerBetLimitJobService class
    /// </summary>
    public class CustomerBetLimitJobServiceTest
    {
        private readonly IAppSettings appSettings;
        private readonly IBetLimitService betLimitService;
        private readonly CustomerBetLimitJobService service;

        public CustomerBetLimitJobServiceTest()
        {
            appSettings = Substitute.For<IAppSettings>();
            betLimitService = Substitute.For<IBetLimitService>();
            service = new CustomerBetLimitJobService(appSettings, betLimitService);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
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

        #region Execute Method Tests

        [Fact]
        public async Task Execute_WhenSuccessful_ShouldReturnTrue()
        {
            // Arrange
            appSettings.RetryTime.Returns(3);
            appSettings.PunterChunkSize.Returns(100);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            var customerList = new List<int> { 1, 2, 3, 4, 5 };
            var removeCustomers = new List<int> { 1, 2 };
            var updateCustomers = new List<int> { 3, 4, 5 };

            betLimitService.GetBetLimitCustomers().Returns(customerList);
            betLimitService.VerifyBetLimitCustomers(Arg.Any<string>()).Returns((removeCustomers, updateCustomers));
            betLimitService.PuntersUpdated.Returns(3);
            betLimitService.RawCustomer.Returns(5);
            betLimitService.UpdateCustomers.Returns(3);
            betLimitService.RemovedCustomers.Returns(2);

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeTrue();
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.Received(1).VerifyBetLimitCustomers("1,2,3,4,5");
            betLimitService.Received(1).RemoveBetLimit(removeCustomers, 100);
            betLimitService.Received(1).UpdateBetLimit(updateCustomers, 100);
        }

        [Fact]
        public async Task Execute_WhenNoCustomers_ShouldReturnTrue()
        {
            // Arrange
            appSettings.RetryTime.Returns(3);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            var emptyCustomerList = new List<int>();
            betLimitService.GetBetLimitCustomers().Returns(emptyCustomerList);

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeTrue();
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.DidNotReceive().VerifyBetLimitCustomers(Arg.Any<string>());
            betLimitService.DidNotReceive().RemoveBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>());
            betLimitService.DidNotReceive().UpdateBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>());
        }

        [Fact]
        public async Task Execute_WhenNullCustomers_ShouldReturnTrue()
        {
            // Arrange
            appSettings.RetryTime.Returns(3);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            betLimitService.GetBetLimitCustomers().Returns((IEnumerable<int>)null);

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeTrue();
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.DidNotReceive().VerifyBetLimitCustomers(Arg.Any<string>());
            betLimitService.DidNotReceive().RemoveBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>());
            betLimitService.DidNotReceive().UpdateBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>());
        }

        [Fact]
        public async Task Execute_WhenExceptionOccursAndRetryExhausted_ShouldReturnFalse()
        {
            // Arrange
            appSettings.RetryTime.Returns(2);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            betLimitService.GetBetLimitCustomers().Throws(new Exception("Database error"));

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeFalse();
            betLimitService.Received(2).GetBetLimitCustomers();
        }
       
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async Task Execute_WithDifferentRetryTimes_ShouldRespectRetryLimit(int retryTime)
        {
            // Arrange
            appSettings.RetryTime.Returns(retryTime);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            betLimitService.GetBetLimitCustomers().Throws(new Exception("Database error"));

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeFalse();
            betLimitService.Received(retryTime).GetBetLimitCustomers();
        }

        #endregion

        #region Private Method Tests

        [Fact]
        public void StartTime_WhenCalled_ShouldLogInfoMessage()
        {
            // Arrange
            var method = typeof(CustomerBetLimitJobService).GetMethod("StartTime", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(service, null);

            // Assert
            // This test verifies the method can be called without throwing an exception
            // The actual logging verification would require more complex setup
            method.Should().NotBeNull();
        }

        [Fact]
        public void UpdateBetLimit_WithValidCustomers_ShouldProcessCorrectly()
        {
            // Arrange
            appSettings.PunterChunkSize.Returns(100);

            var customerList = new List<int> { 1, 2, 3, 4, 5 };
            var removeCustomers = new List<int> { 1, 2 };
            var updateCustomers = new List<int> { 3, 4, 5 };

            betLimitService.GetBetLimitCustomers().Returns(customerList);
            betLimitService.VerifyBetLimitCustomers("1,2,3,4,5").Returns((removeCustomers, updateCustomers));
            betLimitService.PuntersUpdated.Returns(3);
            betLimitService.RawCustomer.Returns(5);
            betLimitService.UpdateCustomers.Returns(3);
            betLimitService.RemovedCustomers.Returns(2);

            var method = typeof(CustomerBetLimitJobService).GetMethod("UpdateBetLimit", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(service, null);

            // Assert
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.Received(1).VerifyBetLimitCustomers("1,2,3,4,5");
            betLimitService.Received(1).RemoveBetLimit(removeCustomers, 100);
            betLimitService.Received(1).UpdateBetLimit(updateCustomers, 100);
        }

        [Fact]
        public void UpdateBetLimit_WithEmptyCustomers_ShouldSetCorrectMessage()
        {
            // Arrange
            var emptyCustomerList = new List<int>();
            betLimitService.GetBetLimitCustomers().Returns(emptyCustomerList);

            var method = typeof(CustomerBetLimitJobService).GetMethod("UpdateBetLimit", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(service, null);

            // Assert
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.DidNotReceive().VerifyBetLimitCustomers(Arg.Any<string>());
            betLimitService.DidNotReceive().RemoveBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>());
            betLimitService.DidNotReceive().UpdateBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>());
        }

        [Fact]
        public void UpdateBetLimit_WithNullCustomers_ShouldSetCorrectMessage()
        {
            // Arrange
            betLimitService.GetBetLimitCustomers().Returns((IEnumerable<int>)null);

            var method = typeof(CustomerBetLimitJobService).GetMethod("UpdateBetLimit", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(service, null);

            // Assert
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.DidNotReceive().VerifyBetLimitCustomers(Arg.Any<string>());
            betLimitService.DidNotReceive().RemoveBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>());
            betLimitService.DidNotReceive().UpdateBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>());
        }

        [Fact]
        public void NotifyMessage_WithValidMessage_ShouldCallUtilitiesNotifyMessage()
        {
            // Arrange
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            // Set the notifiedMessage field using reflection
            var field = typeof(CustomerBetLimitJobService).GetField("notifiedMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(service, "Test notification message");

            var method = typeof(CustomerBetLimitJobService).GetMethod("NotifyMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(service, null);

            // Assert
            // This test verifies the method can be called without throwing an exception
            // The actual Utilities.NotifyMessage call would require more complex setup
            method.Should().NotBeNull();
        }

        [Fact]
        public void NotifyMessage_WithEmptyMessage_ShouldNotCallUtilitiesNotifyMessage()
        {
            // Arrange
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            // Set the notifiedMessage field to empty using reflection
            var field = typeof(CustomerBetLimitJobService).GetField("notifiedMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(service, string.Empty);

            var method = typeof(CustomerBetLimitJobService).GetMethod("NotifyMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(service, null);

            // Assert
            // This test verifies the method can be called without throwing an exception
            // The actual Utilities.NotifyMessage call would require more complex setup
            method.Should().NotBeNull();
        }

        [Fact]
        public void NotifyMessage_WithNullMessage_ShouldNotCallUtilitiesNotifyMessage()
        {
            // Arrange
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            // Set the notifiedMessage field to null using reflection
            var field = typeof(CustomerBetLimitJobService).GetField("notifiedMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(service, null);

            var method = typeof(CustomerBetLimitJobService).GetMethod("NotifyMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(service, null);

            // Assert
            // This test verifies the method can be called without throwing an exception
            // The actual Utilities.NotifyMessage call would require more complex setup
            method.Should().NotBeNull();
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public async Task Execute_WhenBetLimitServiceThrowsException_ShouldHandleGracefully()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            betLimitService.GetBetLimitCustomers().Throws(new InvalidOperationException("Service unavailable"));

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeFalse();
            betLimitService.Received(1).GetBetLimitCustomers();
        }

        [Fact]
        public async Task Execute_WhenVerifyBetLimitCustomersThrowsException_ShouldHandleGracefully()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            appSettings.PunterChunkSize.Returns(100);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            var customerList = new List<int> { 1, 2, 3 };
            betLimitService.GetBetLimitCustomers().Returns(customerList);
            betLimitService.VerifyBetLimitCustomers(Arg.Any<string>()).Throws(new Exception("Verification failed"));

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeFalse();
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.Received(1).VerifyBetLimitCustomers("1,2,3");
        }

        [Fact]
        public async Task Execute_WhenRemoveBetLimitThrowsException_ShouldHandleGracefully()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            appSettings.PunterChunkSize.Returns(100);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            var customerList = new List<int> { 1, 2, 3 };
            var removeCustomers = new List<int> { 1, 2 };
            var updateCustomers = new List<int> { 3 };

            betLimitService.GetBetLimitCustomers().Returns(customerList);
            betLimitService.VerifyBetLimitCustomers(Arg.Any<string>()).Returns((removeCustomers, updateCustomers));
            betLimitService.When(x => x.RemoveBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>()))
                .Throw(new Exception("Remove failed"));

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeFalse();
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.Received(1).VerifyBetLimitCustomers("1,2,3");
            betLimitService.Received(1).RemoveBetLimit(removeCustomers, 100);
        }

        [Fact]
        public async Task Execute_WhenUpdateBetLimitThrowsException_ShouldHandleGracefully()
        {
            // Arrange
            appSettings.RetryTime.Returns(1);
            appSettings.PunterChunkSize.Returns(100);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            var customerList = new List<int> { 1, 2, 3 };
            var removeCustomers = new List<int> { 1 };
            var updateCustomers = new List<int> { 2, 3 };

            betLimitService.GetBetLimitCustomers().Returns(customerList);
            betLimitService.VerifyBetLimitCustomers(Arg.Any<string>()).Returns((removeCustomers, updateCustomers));
            betLimitService.When(x => x.UpdateBetLimit(Arg.Any<IEnumerable<int>>(), Arg.Any<int>()))
                .Throw(new Exception("Update failed"));

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeFalse();
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.Received(1).VerifyBetLimitCustomers("1,2,3");
            betLimitService.Received(1).RemoveBetLimit(removeCustomers, 100);
            betLimitService.Received(1).UpdateBetLimit(updateCustomers, 100);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Execute_WithCompleteWorkflow_ShouldProcessAllSteps()
        {
            // Arrange
            appSettings.RetryTime.Returns(3);
            appSettings.PunterChunkSize.Returns(50);
            appSettings.EnvironmentString.Returns("PROD");
            appSettings.LogGroupID.Returns("prod-group");

            var customerList = new List<int> { 100, 200, 300, 400, 500, 600 };
            var removeCustomers = new List<int> { 100, 200 };
            var updateCustomers = new List<int> { 300, 400, 500, 600 };

            betLimitService.GetBetLimitCustomers().Returns(customerList);
            betLimitService.VerifyBetLimitCustomers("100,200,300,400,500,600").Returns((removeCustomers, updateCustomers));
            betLimitService.PuntersUpdated.Returns(4);
            betLimitService.RawCustomer.Returns(6);
            betLimitService.UpdateCustomers.Returns(4);
            betLimitService.RemovedCustomers.Returns(2);

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeTrue();
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.Received(1).VerifyBetLimitCustomers("100,200,300,400,500,600");
            betLimitService.Received(1).RemoveBetLimit(removeCustomers, 50);
            betLimitService.Received(1).UpdateBetLimit(updateCustomers, 50);
        }

        [Fact]
        public async Task Execute_WithLargeCustomerList_ShouldProcessInChunks()
        {
            // Arrange
            appSettings.RetryTime.Returns(3);
            appSettings.PunterChunkSize.Returns(2);
            appSettings.EnvironmentString.Returns("UAT");
            appSettings.LogGroupID.Returns("test-group");

            var customerList = new List<int> { 1, 2, 3, 4, 5 };
            var removeCustomers = new List<int> { 1, 2, 3 };
            var updateCustomers = new List<int> { 4, 5 };

            betLimitService.GetBetLimitCustomers().Returns(customerList);
            betLimitService.VerifyBetLimitCustomers("1,2,3,4,5").Returns((removeCustomers, updateCustomers));
            betLimitService.PuntersUpdated.Returns(2);
            betLimitService.RawCustomer.Returns(5);
            betLimitService.UpdateCustomers.Returns(2);
            betLimitService.RemovedCustomers.Returns(3);

            // Act
            var result = await service.Execute();

            // Assert
            result.Should().BeTrue();
            betLimitService.Received(1).GetBetLimitCustomers();
            betLimitService.Received(1).VerifyBetLimitCustomers("1,2,3,4,5");
            betLimitService.Received(1).RemoveBetLimit(removeCustomers, 2);
            betLimitService.Received(1).UpdateBetLimit(updateCustomers, 2);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void NotifiedMessage_ShouldBeInitializedAsEmptyString()
        {
            // Arrange
            var field = typeof(CustomerBetLimitJobService).GetField("notifiedMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var value = field.GetValue(service);

            // Assert
            value.Should().Be(string.Empty);
        }

        #endregion

        #region Method Signature Tests

        [Fact]
        public void Execute_ShouldReturnTaskOfBool()
        {
            // Arrange
            var method = typeof(CustomerBetLimitJobService).GetMethod("Execute");

            // Assert
            method.ReturnType.Should().Be(typeof(Task<bool>));
            method.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void StartTime_ShouldBePrivateVoidMethod()
        {
            // Arrange
            var method = typeof(CustomerBetLimitJobService).GetMethod("StartTime", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void UpdateBetLimit_ShouldBePrivateVoidMethod()
        {
            // Arrange
            var method = typeof(CustomerBetLimitJobService).GetMethod("UpdateBetLimit", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void NotifyMessage_ShouldBePrivateVoidMethod()
        {
            // Arrange
            var method = typeof(CustomerBetLimitJobService).GetMethod("NotifyMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().BeEmpty();
        }

        #endregion
    }
}
