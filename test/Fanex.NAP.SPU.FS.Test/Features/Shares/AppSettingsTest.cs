namespace Fanex.NAP.SPU.VR.Tests.Features.Shares
{
    using Fanex.NAP.SPU.VR.JobService;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Unit tests for AppSettings class
    /// </summary>
    public class AppSettingsTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalledWithValidConfiguration_ShouldInitializeCorrectly()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Should().NotBeNull();
        }
      
        #endregion

        #region Environment Property Tests

        [Theory]
        [InlineData("LOCAL", 1)]
        [InlineData("UAT", 2)]
        [InlineData("PROD", 3)]
        [InlineData("PRODUCTION", 3)]
        [InlineData("TEST", 3)]
        [InlineData("", 3)]
        [InlineData(null, 3)]
        public void Environment_WhenEnvironmentStringIsSet_ShouldReturnCorrectValue(string environmentString, int expectedValue)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["Environment"].Returns(environmentString);

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Environment.Should().Be(expectedValue);
        }

        [Fact]
        public void EnvironmentString_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            var expectedEnvironment = "TEST";
            configuration["Environment"].Returns(expectedEnvironment);

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.EnvironmentString.Should().Be(expectedEnvironment);
        }

        #endregion

        #region Basic Property Tests       

        [Theory]
        [InlineData(100)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void PunterChunkSize_WhenSet_ShouldReturnCorrectValue(int punterChunkSize)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["PunterChunkSize"].Returns(punterChunkSize.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.PunterChunkSize.Should().Be(punterChunkSize);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void MailingListIdAgentAjust_WhenSet_ShouldReturnCorrectValue(int mailingListId)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["MailingListIdAgentAjust"].Returns(mailingListId.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.MailingListIdAgentAjust.Should().Be(mailingListId);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void RetryTime_WhenSet_ShouldReturnCorrectValue(int retryTime)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["RetryTime"].Returns(retryTime.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.RetryTime.Should().Be(retryTime);
        }
       
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsTest_WhenSet_ShouldReturnCorrectValue(bool isTest)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["IsTest"].Returns(isTest.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.IsTest.Should().Be(isTest);
        }       

        #endregion

        #region Adjust3M Property Tests
       
        [Theory]
        [InlineData(50)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Adjust3MPunterChunkSize_WhenSet_ShouldReturnCorrectValue(int chunkSize)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["Adjust3MPunterChunkSize"].Returns(chunkSize.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Adjust3MPunterChunkSize.Should().Be(chunkSize);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Adjust3MNewVirtualMultiplierForSuperProPunter_WhenSet_ShouldReturnCorrectValue(int multiplier)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["Adjust3MNewVirtualMultiplierForSuperProPunter"].Returns(multiplier.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Adjust3MNewVirtualMultiplierForSuperProPunter.Should().Be(multiplier);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Adjust3MNewVirtualMultiplierForWinning_WhenSet_ShouldReturnCorrectValue(int multiplier)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["Adjust3MNewVirtualMultiplierForWinning"].Returns(multiplier.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Adjust3MNewVirtualMultiplierForWinning.Should().Be(multiplier);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Adjust3MVirtualRateThresholdForWinning_WhenSet_ShouldReturnCorrectValue(int threshold)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["Adjust3MVirtualRateThresholdForWinning"].Returns(threshold.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Adjust3MVirtualRateThresholdForWinning.Should().Be(threshold);
        }

        [Theory]
        [InlineData(999)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Adjust3MDummyUser_WhenSet_ShouldReturnCorrectValue(int dummyUser)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["Adjust3MDummyUser"].Returns(dummyUser.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Adjust3MDummyUser.Should().Be(dummyUser);
        }

        [Theory]
        [InlineData(123)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Adjust3MMailingListId_WhenSet_ShouldReturnCorrectValue(int mailingListId)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["Adjust3MMailingListId"].Returns(mailingListId.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Adjust3MMailingListId.Should().Be(mailingListId);
        }

        [Theory]
        [InlineData(456)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void MailingListIdToSPU_WhenSet_ShouldReturnCorrectValue(int mailingListId)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["MailingListIdToSPU"].Returns(mailingListId.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.MailingListIdToSPU.Should().Be(mailingListId);
        }

        #endregion

        #region SapaSportDangerLevel Property Tests

        [Theory]
        [InlineData(5)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void RetryTimesSapaSportDangerLevelForCheckingDataReady_WhenSet_ShouldReturnCorrectValue(int retryTimes)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["SapaSportDangerLevel:RetryTimesForCheckingDataReady"].Returns(retryTimes.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady.Should().Be(retryTimes);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void RetryTimesSapaSportDangerLevelForUpdateDangerLevel_WhenSet_ShouldReturnCorrectValue(int retryTimes)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["SapaSportDangerLevel:RetryTimesForUpdateDangerLevel"].Returns(retryTimes.ToString());

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel.Should().Be(retryTimes);
        }

        #endregion

        #region GetValue Method Tests       

        [Theory]
        [InlineData("IntKey", "42", 42)]
        [InlineData("IntKey", "0", 0)]
        [InlineData("IntKey", "-1", -1)]
        [InlineData("IntKey", "2147483647", int.MaxValue)]
        [InlineData("IntKey", "-2147483648", int.MinValue)]
        public void GetValue_WhenCalledWithIntType_ShouldReturnCorrectValue(string key, string value, int expectedValue)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration[key].Returns(value);

            // Act
            var appSettings = new AppSettings(configuration);
            var result = appSettings.GetValue<int>(key);

            // Assert
            result.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("BoolKey", "true", true)]
        [InlineData("BoolKey", "false", false)]
        [InlineData("BoolKey", "True", true)]
        [InlineData("BoolKey", "False", false)]
        [InlineData("BoolKey", "TRUE", true)]
        [InlineData("BoolKey", "FALSE", false)]
        public void GetValue_WhenCalledWithBoolType_ShouldReturnCorrectValue(string key, string value, bool expectedValue)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration[key].Returns(value);

            // Act
            var appSettings = new AppSettings(configuration);
            var result = appSettings.GetValue<bool>(key);

            // Assert
            result.Should().Be(expectedValue);
        }

        [Fact]
        public void GetValue_WhenCalledWithInvalidIntValue_ShouldThrowInvalidCastException()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["InvalidIntKey"].Returns("not-a-number");

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            var action = () => appSettings.GetValue<int>("InvalidIntKey");
            action.Should().Throw<InvalidCastException>()
                .WithMessage("Key: InvalidIntKey*");
        }

        [Fact]
        public void GetValue_WhenCalledWithInvalidBoolValue_ShouldThrowInvalidCastException()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["InvalidBoolKey"].Returns("not-a-bool");

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            var action = () => appSettings.GetValue<bool>("InvalidBoolKey");
            action.Should().Throw<InvalidCastException>()
                .WithMessage("Key: InvalidBoolKey*");
        }

        #endregion      

        #region Edge Cases and Error Handling Tests      

        [Fact]
        public void GetSection_WhenConfigurationThrowsException_ShouldPropagateException()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration.When(x => x.GetSection("TestSection")).Do(x => throw new Exception("Configuration error"));

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            var action = () => appSettings.GetSection<object>("TestSection");
            action.Should().Throw<Exception>().WithMessage("Configuration error");
        }

        [Fact]
        public void GetValue_WhenTypeConverterThrowsException_ShouldThrowInvalidCastException()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            SetupDefaultConfiguration(configuration);
            configuration["TestKey"].Returns("invalid-value");

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            var action = () => appSettings.GetValue<DateTime>("TestKey");
            action.Should().Throw<InvalidCastException>()
                .WithMessage("Key: TestKey*");
        }

        #endregion

        #region Helper Methods

        private static void SetupDefaultConfiguration(IConfiguration configuration)
        {
            // Setup default configuration values for all required keys
            configuration["Environment"].Returns("LOCAL");
            configuration["PunterChunkSize"].Returns("100");
            configuration["SPName"].Returns("TestSP");
            configuration["MailingListIdAgentAjust"].Returns("1");
            configuration["RetryTime"].Returns("3");
            configuration["Sentry:Dsn"].Returns("https://sentry.io/test");
            configuration["Adjust3MExecutedDate"].Returns("2023-01-01");
            configuration["Adjust3MPunterChunkSize"].Returns("50");
            configuration["Adjust3MNewVirtualMultiplierForSuperProPunter"].Returns("2");
            configuration["Adjust3MNewVirtualMultiplierForWinning"].Returns("1");
            configuration["Adjust3MVirtualRateThresholdForWinning"].Returns("1");
            configuration["Adjust3MSiteIds"].Returns("1,2,3");
            configuration["Adjust3MSuperIds"].Returns("100,200,300");
            configuration["Adjust3MIP"].Returns("192.168.1.1");
            configuration["Adjust3MDummyUser"].Returns("999");
            configuration["Adjust3MMailingListId"].Returns("123");
            configuration["MailingListIdToSPU"].Returns("456");
            configuration["SapaSportDangerLevel:RetryTimesForCheckingDataReady"].Returns("5");
            configuration["SapaSportDangerLevel:RetryTimesForUpdateDangerLevel"].Returns("3");
            configuration["IsTest"].Returns("true");
            configuration["LogGroupID"].Returns("TestLogGroup");
        }

        #endregion
    }
}
