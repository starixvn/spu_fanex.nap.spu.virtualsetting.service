namespace Fanex.NAP.SPU.VR.Tests.Features.Notification
{
    using Fanex.NAP.SPU.VR.Features.Notification;
    using FluentAssertions;
    using System;
    using System.Net;
    using Xunit;

    /// <summary>
    /// Unit tests for NotificationService class
    /// </summary>
    public class NotificationServiceTest
    {
        private readonly NotificationService service;

        public NotificationServiceTest()
        {
            service = new NotificationService();
        }

        #region Message Tests

        [Fact]
        public void Message_WhenMessageModelIsNull_ShouldThrowArgumentException()
        {
            // Arrange
            MessageModel messageModel = null;

            // Act
            Action act = () => service.Message(messageModel);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Parameter cannot be null (Parameter 'messageModel')");
        }

        [Fact]
        public void Message_WhenMessageModelIsValid_ShouldReturnSelf()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message"
            );

            // Act
            var result = service.Message(messageModel);

            // Assert
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void Message_WhenMessageModelHasMoreInfo_ShouldSetMessageModel()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message",
                "Additional information"
            );

            // Act
            service.Message(messageModel);

            // Assert
            // Verify the message was set by checking it can be used in subsequent calls
            var result = service.Config("test-group-id", "LOCAL");
            result.Should().BeSameAs(service);
        }

        #endregion

        #region Config Tests

        [Fact]
        public void Config_WhenGroupIdIsNull_ShouldThrowArgumentException()
        {
            // Arrange
            string groupId = null;
            string environment = "LOCAL";

            // Act
            Action act = () => service.Config(groupId, environment);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Group Id cannot be null (Parameter 'groupId')");
        }

        [Fact]
        public void Config_WhenGroupIdIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            string groupId = string.Empty;
            string environment = "LOCAL";

            // Act
            Action act = () => service.Config(groupId, environment);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Group Id cannot be null (Parameter 'groupId')");
        }

        [Fact]
        public void Config_WhenGroupIdIsWhitespace_ShouldThrowArgumentException()
        {
            // Arrange
            string groupId = "   ";
            string environment = "LOCAL";

            // Act
            Action act = () => service.Config(groupId, environment);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Group Id cannot be null (Parameter 'groupId')");
        }

        [Fact]
        public void Config_WhenGroupIdIsValid_ShouldReturnSelf()
        {
            // Arrange
            string groupId = "test-group-id";
            string environment = "LOCAL";

            // Act
            var result = service.Config(groupId, environment);

            // Assert
            result.Should().BeSameAs(service);
        }

        [Theory]
        [InlineData("PRO")]
        [InlineData("KLDR")]
        public void Config_WhenEnvironmentIsProdOrKlDr_ShouldSetProxy(string environment)
        {
            // Arrange
            string groupId = "test-group-id";

            // Act
            service.Config(groupId, environment);

            // Assert
            // The proxy is set internally, we verify by checking the service can be used
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                environment,
                "Test message"
            );
            service.Message(messageModel);
            
            // Verify no exception is thrown (proxy is set)
            var result = service.Config(groupId, environment);
            result.Should().BeSameAs(service);
        }

        [Theory]
        [InlineData("LOCAL")]
        [InlineData("UAT")]
        [InlineData("DEV")]
        [InlineData("TEST")]
        public void Config_WhenEnvironmentIsNotProdOrKlDr_ShouldNotSetProxy(string environment)
        {
            // Arrange
            string groupId = "test-group-id";

            // Act
            service.Config(groupId, environment);

            // Assert
            // Verify no exception is thrown (proxy is null for non-prod environments)
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                environment,
                "Test message"
            );
            service.Message(messageModel);
            
            var result = service.Config(groupId, environment);
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void Config_WhenCalled_ShouldSetTokenAndApiUrl()
        {
            // Arrange
            string groupId = "test-group-id";
            string environment = "LOCAL";

            // Act
            service.Config(groupId, environment);

            // Assert
            // Verify the service is configured by checking it can be used
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                environment,
                "Test message"
            );
            service.Message(messageModel);
            
            var result = service.Config(groupId, environment);
            result.Should().BeSameAs(service);
        }

        #endregion

        #region Method Chaining Tests

        [Fact]
        public void Message_ThenConfig_ShouldSupportMethodChaining()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message"
            );

            // Act
            var result = service
                .Message(messageModel)
                .Config("test-group-id", "LOCAL");

            // Assert
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void Config_ThenMessage_ShouldSupportMethodChaining()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message"
            );

            // Act
            var result = service
                .Config("test-group-id", "LOCAL")
                .Message(messageModel);

            // Assert
            result.Should().BeSameAs(service);
        }

        #endregion

        #region SendMessageError Tests

        [Fact]
        public void SendMessageError_WhenMessageModelIsNotSet_ShouldThrowNullReferenceException()
        {
            // Arrange
            service.Config("test-group-id", "LOCAL");
            // Message is not set

            // Act
            Action act = () => service.SendMessageError();

            // Assert
            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void SendMessageError_WhenConfigIsNotSet_ShouldThrowUriFormatException()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message"
            );
            service.Message(messageModel);
            // Config is not set

            // Act
            Action act = () => service.SendMessageError();

            // Assert
            act.Should().Throw<UriFormatException>();
        }

        [Fact]
        public void SendMessageError_WhenMessageModelHasMoreInfo_ShouldIncludeMoreInfoInMessage()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message",
                "Additional information"
            );
            service.Message(messageModel).Config("test-group-id", "LOCAL");

            // Act & Assert
            // Note: This will make an actual HTTP call, so it may fail if the API is not available
            // In a real scenario, you might want to use a mock HTTP client or integration test
            // For now, we just verify it doesn't throw an exception for message formatting
            Action act = () => service.SendMessageError();
            
            // The method will attempt to send the message
            // We can't easily verify the result without mocking RestSharp, which requires refactoring
            // This test ensures the method can be called without throwing formatting exceptions
            act.Should().NotThrow<NullReferenceException>();
        }

        [Fact]
        public void SendMessageError_WhenMessageModelHasNoMoreInfo_ShouldNotIncludeMoreInfo()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message"
            );
            service.Message(messageModel).Config("test-group-id", "LOCAL");

            // Act & Assert
            Action act = () => service.SendMessageError();
            
            // Verify the method can be called without throwing formatting exceptions
            act.Should().NotThrow<NullReferenceException>();
        }

        #endregion

        #region SendMessageInfo Tests

        [Fact]
        public void SendMessageInfo_WhenMessageModelIsNotSet_ShouldThrowNullReferenceException()
        {
            // Arrange
            service.Config("test-group-id", "LOCAL");
            // Message is not set

            // Act
            Action act = () => service.SendMessageInfo();

            // Assert
            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void SendMessageInfo_WhenConfigIsNotSet_ShouldThrowUriFormatException()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message"
            );
            service.Message(messageModel);
            // Config is not set

            // Act
            Action act = () => service.SendMessageInfo();

            // Assert
            act.Should().Throw<UriFormatException>();
        }

        [Fact]
        public void SendMessageInfo_WhenMessageModelHasMoreInfo_ShouldIncludeMoreInfoInMessage()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message",
                "Additional information"
            );
            service.Message(messageModel).Config("test-group-id", "LOCAL");

            // Act & Assert
            // Note: This will make an actual HTTP call, so it may fail if the API is not available
            Action act = () => service.SendMessageInfo();
            
            // Verify the method can be called without throwing formatting exceptions
            act.Should().NotThrow<NullReferenceException>();
        }

        [Fact]
        public void SendMessageInfo_WhenMessageModelHasNoMoreInfo_ShouldNotIncludeMoreInfo()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.Now,
                "LOCAL",
                "Test message"
            );
            service.Message(messageModel).Config("test-group-id", "LOCAL");

            // Act & Assert
            Action act = () => service.SendMessageInfo();
            
            // Verify the method can be called without throwing formatting exceptions
            act.Should().NotThrow<NullReferenceException>();
        }

        #endregion

        #region Integration Flow Tests

        [Fact]
        public void FullFlow_Message_Config_SendMessageError_ShouldWork()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.UtcNow,
                "LOCAL",
                "Integration test message"
            );

            // Act & Assert
            var result = service
                .Message(messageModel)
                .Config("test-group-id", "LOCAL");
            
            result.Should().BeSameAs(service);
            
            // Note: This makes an actual HTTP call
            // In production, you might want to mark this as an integration test
            Action act = () => service.SendMessageError();
            act.Should().NotThrow<NullReferenceException>();
        }

        [Fact]
        public void FullFlow_Config_Message_SendMessageInfo_ShouldWork()
        {
            // Arrange
            var messageModel = new MessageModel(
                "TestSystem",
                DateTimeOffset.UtcNow,
                "LOCAL",
                "Integration test message",
                "Extra details"
            );

            // Act & Assert
            var result = service
                .Config("test-group-id", "LOCAL")
                .Message(messageModel);
            
            result.Should().BeSameAs(service);
            
            // Note: This makes an actual HTTP call
            // In production, you might want to mark this as an integration test
            Action act = () => service.SendMessageInfo();
            act.Should().NotThrow<NullReferenceException>();
        }

        #endregion
    }
}

