namespace Fanex.NAP.SPU.VR.Tests.Features.Shares
{
    using Fanex.NAP.SPU.VR.JobService.Controllers;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for JobController class
    /// </summary>
    public class JobControllerTest
    {
        private readonly ICustomerBetLimitJobService customerBetLimitJobService;
        private readonly ISpecialLicenseeJobService specialLicenseeJobService;
        private readonly IAgentJobService agentJobService;
        private readonly IMemberBigStakeJobService memberBigStakeJobService;
        private readonly IAdjust3MJobService adjust3MJobService;
        private readonly ISapaSportDangerLevelJobService sapaSportDangerLevelJobService;
        private readonly JobController controller;

        public JobControllerTest()
        {
            customerBetLimitJobService = Substitute.For<ICustomerBetLimitJobService>();
            specialLicenseeJobService = Substitute.For<ISpecialLicenseeJobService>();
            agentJobService = Substitute.For<IAgentJobService>();
            memberBigStakeJobService = Substitute.For<IMemberBigStakeJobService>();
            adjust3MJobService = Substitute.For<IAdjust3MJobService>();
            sapaSportDangerLevelJobService = Substitute.For<ISapaSportDangerLevelJobService>();
            
            controller = new JobController(
                customerBetLimitJobService,
                specialLicenseeJobService,
                agentJobService,
                memberBigStakeJobService,
                adjust3MJobService,
                sapaSportDangerLevelJobService);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalledWithValidDependencies_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockCustomerBetLimitJobService = Substitute.For<ICustomerBetLimitJobService>();
            var mockSpecialLicenseeJobService = Substitute.For<ISpecialLicenseeJobService>();
            var mockAgentJobService = Substitute.For<IAgentJobService>();
            var mockMemberBigStakeJobService = Substitute.For<IMemberBigStakeJobService>();
            var mockAdjust3MJobService = Substitute.For<IAdjust3MJobService>();
            var mockSapaSportDangerLevelJobService = Substitute.For<ISapaSportDangerLevelJobService>();

            // Act
            var controller = new JobController(
                mockCustomerBetLimitJobService,
                mockSpecialLicenseeJobService,
                mockAgentJobService,
                mockMemberBigStakeJobService,
                mockAdjust3MJobService,
                mockSapaSportDangerLevelJobService);

            // Assert
            controller.Should().NotBeNull();
        }
      
        #endregion

        #region BetLimit Endpoint Tests

        [Fact]
        public async Task BetLimit_WhenCalledWithValidJobName_ShouldReturnTrue()
        {
            // Arrange
            var jobName = "TestBetLimitJob";
            customerBetLimitJobService.Execute().Returns(Task.FromResult(true));

            // Act
            var result = await controller.BetLimit(jobName);

            // Assert
            result.Should().BeTrue();
            customerBetLimitJobService.Received(1).Execute();
        }

        [Fact]
        public async Task BetLimit_WhenCalledWithEmptyJobName_ShouldReturnResult()
        {
            // Arrange
            var jobName = "";
            customerBetLimitJobService.Execute().Returns(Task.FromResult(false));

            // Act
            var result = await controller.BetLimit(jobName);

            // Assert
            result.Should().BeFalse();
            customerBetLimitJobService.Received(1).Execute();
        }

        [Fact]
        public async Task BetLimit_WhenCalledWithNullJobName_ShouldReturnResult()
        {
            // Arrange
            string jobName = null;
            customerBetLimitJobService.Execute().Returns(Task.FromResult(true));

            // Act
            var result = await controller.BetLimit(jobName);

            // Assert
            result.Should().BeTrue();
            customerBetLimitJobService.Received(1).Execute();
        }
      
        #endregion

        #region MemberBigStake Endpoint Tests

        [Fact]
        public async Task MemberBigStake_WhenCalledWithValidJobName_ShouldReturnTrue()
        {
            // Arrange
            var jobName = "TestMemberBigStakeJob";
            memberBigStakeJobService.Execute().Returns(Task.FromResult(true));

            // Act
            var result = await controller.MemberBigStake(jobName);

            // Assert
            result.Should().BeTrue();
            memberBigStakeJobService.Received(1).Execute();
        }

        [Fact]
        public async Task MemberBigStake_WhenCalledWithEmptyJobName_ShouldReturnResult()
        {
            // Arrange
            var jobName = "";
            memberBigStakeJobService.Execute().Returns(Task.FromResult(false));

            // Act
            var result = await controller.MemberBigStake(jobName);

            // Assert
            result.Should().BeFalse();
            memberBigStakeJobService.Received(1).Execute();
        }       

        #endregion

        #region SpecialLicensee Endpoint Tests

        [Fact]
        public async Task SpecialLicensee_WhenCalledWithValidJobName_ShouldReturnTrue()
        {
            // Arrange
            var jobName = "TestSpecialLicenseeJob";
            specialLicenseeJobService.Execute().Returns(Task.FromResult(true));

            // Act
            var result = await controller.SpecialLicensee(jobName);

            // Assert
            result.Should().BeTrue();
            specialLicenseeJobService.Received(1).Execute();
        }

        [Fact]
        public async Task SpecialLicensee_WhenCalledWithEmptyJobName_ShouldReturnResult()
        {
            // Arrange
            var jobName = "";
            specialLicenseeJobService.Execute().Returns(Task.FromResult(false));

            // Act
            var result = await controller.SpecialLicensee(jobName);

            // Assert
            result.Should().BeFalse();
            specialLicenseeJobService.Received(1).Execute();
        }
     
        #endregion

        #region Agent Endpoint Tests

        [Fact]
        public async Task Agent_WhenCalledWithValidJobName_ShouldReturnTrue()
        {
            // Arrange
            var jobName = "TestAgentJob";
            agentJobService.Execute().Returns(Task.FromResult(true));

            // Act
            var result = await controller.Agent(jobName);

            // Assert
            result.Should().BeTrue();
            agentJobService.Received(1).Execute();
        }

        [Fact]
        public async Task Agent_WhenCalledWithEmptyJobName_ShouldReturnResult()
        {
            // Arrange
            var jobName = "";
            agentJobService.Execute().Returns(Task.FromResult(false));

            // Act
            var result = await controller.Agent(jobName);

            // Assert
            result.Should().BeFalse();
            agentJobService.Received(1).Execute();
        }        

        #endregion

        #region AdjustVRSetting Endpoint Tests

        [Fact]
        public async Task AdjustVRSetting_WhenCalledWithValidJobName_ShouldReturnTrue()
        {
            // Arrange
            var jobName = "TestAdjustVRSettingJob";
            adjust3MJobService.Execute().Returns(Task.FromResult(true));

            // Act
            var result = await controller.AdjustVRSetting(jobName);

            // Assert
            result.Should().BeTrue();
            adjust3MJobService.Received(1).Execute();
        }

        [Fact]
        public async Task AdjustVRSetting_WhenCalledWithEmptyJobName_ShouldReturnResult()
        {
            // Arrange
            var jobName = "";
            adjust3MJobService.Execute().Returns(Task.FromResult(false));

            // Act
            var result = await controller.AdjustVRSetting(jobName);

            // Assert
            result.Should().BeFalse();
            adjust3MJobService.Received(1).Execute();
        }
      
        #endregion

        #region WriteLog Endpoint Tests

        [Fact]
        public void WriteLog_WhenCalledWithValidMessage_ShouldNotThrowException()
        {
            // Arrange
            var message = "Test log message";

            // Act & Assert
            var action = () => controller.WriteLog(message);
            action.Should().NotThrow();
        }

        [Fact]
        public void WriteLog_WhenCalledWithEmptyMessage_ShouldNotThrowException()
        {
            // Arrange
            var message = "";

            // Act & Assert
            var action = () => controller.WriteLog(message);
            action.Should().NotThrow();
        }

        [Fact]
        public void WriteLog_WhenCalledWithNullMessage_ShouldNotThrowException()
        {
            // Arrange
            string message = null;

            // Act & Assert
            var action = () => controller.WriteLog(message);
            action.Should().NotThrow();
        }

        [Fact]
        public void WriteLog_WhenCalledWithSpecialCharacters_ShouldNotThrowException()
        {
            // Arrange
            var message = "Test message with special chars: & < > \" ' \n \r \t";

            // Act & Assert
            var action = () => controller.WriteLog(message);
            action.Should().NotThrow();
        }

        [Fact]
        public void WriteLog_WhenCalledWithLongMessage_ShouldNotThrowException()
        {
            // Arrange
            var message = new string('A', 10000);

            // Act & Assert
            var action = () => controller.WriteLog(message);
            action.Should().NotThrow();
        }

        #endregion

        #region SapaSportDangerLevel Endpoint Tests

        [Fact]
        public async Task SapaSportDangerLevel_WhenCalledWithValidBatchSize_ShouldReturnTrue()
        {
            // Arrange
            var batchSize = 100;
            sapaSportDangerLevelJobService.Execute(batchSize).Returns(Task.FromResult(true));

            // Act
            var result = await controller.SapaSportDangerLevel(batchSize);

            // Assert
            result.Should().BeTrue();
            sapaSportDangerLevelJobService.Received(1).Execute(batchSize);
        }

        [Fact]
        public async Task SapaSportDangerLevel_WhenCalledWithZeroBatchSize_ShouldReturnResult()
        {
            // Arrange
            var batchSize = 0;
            sapaSportDangerLevelJobService.Execute(batchSize).Returns(Task.FromResult(false));

            // Act
            var result = await controller.SapaSportDangerLevel(batchSize);

            // Assert
            result.Should().BeFalse();
            sapaSportDangerLevelJobService.Received(1).Execute(batchSize);
        }

        [Fact]
        public async Task SapaSportDangerLevel_WhenCalledWithNegativeBatchSize_ShouldReturnResult()
        {
            // Arrange
            var batchSize = -1;
            sapaSportDangerLevelJobService.Execute(batchSize).Returns(Task.FromResult(false));

            // Act
            var result = await controller.SapaSportDangerLevel(batchSize);

            // Assert
            result.Should().BeFalse();
            sapaSportDangerLevelJobService.Received(1).Execute(batchSize);
        }

        [Fact]
        public async Task SapaSportDangerLevel_WhenCalledWithMaxIntBatchSize_ShouldReturnResult()
        {
            // Arrange
            var batchSize = int.MaxValue;
            sapaSportDangerLevelJobService.Execute(batchSize).Returns(Task.FromResult(true));

            // Act
            var result = await controller.SapaSportDangerLevel(batchSize);

            // Assert
            result.Should().BeTrue();
            sapaSportDangerLevelJobService.Received(1).Execute(batchSize);
        }
       
        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public async Task BetLimit_WhenCalledMultipleTimes_ShouldCallServiceMultipleTimes()
        {
            // Arrange
            var jobName = "TestJob";
            customerBetLimitJobService.Execute().Returns(Task.FromResult(true));

            // Act
            await controller.BetLimit(jobName);
            await controller.BetLimit(jobName);
            await controller.BetLimit(jobName);

            // Assert
            customerBetLimitJobService.Received(3).Execute();
        }

        [Fact]
        public async Task AllEndpoints_WhenCalledConcurrently_ShouldNotInterfere()
        {
            // Arrange
            var jobName = "TestJob";
            var batchSize = 100;
            
            customerBetLimitJobService.Execute().Returns(Task.FromResult(true));
            memberBigStakeJobService.Execute().Returns(Task.FromResult(true));
            specialLicenseeJobService.Execute().Returns(Task.FromResult(true));
            agentJobService.Execute().Returns(Task.FromResult(true));
            adjust3MJobService.Execute().Returns(Task.FromResult(true));
            sapaSportDangerLevelJobService.Execute(batchSize).Returns(Task.FromResult(true));

            // Act
            var tasks = new[]
            {
                controller.BetLimit(jobName),
                controller.MemberBigStake(jobName),
                controller.SpecialLicensee(jobName),
                controller.Agent(jobName),
                controller.AdjustVRSetting(jobName),
                controller.SapaSportDangerLevel(batchSize)
            };

            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().AllBeEquivalentTo(true);
            customerBetLimitJobService.Received(1).Execute();
            memberBigStakeJobService.Received(1).Execute();
            specialLicenseeJobService.Received(1).Execute();
            agentJobService.Received(1).Execute();
            adjust3MJobService.Received(1).Execute();
            sapaSportDangerLevelJobService.Received(1).Execute(batchSize);
        }

        [Fact]
        public void WriteLog_WhenCalledMultipleTimes_ShouldNotThrowException()
        {
            // Arrange
            var messages = new[] { "Message1", "Message2", "Message3" };

            // Act & Assert
            foreach (var message in messages)
            {
                var action = () => controller.WriteLog(message);
                action.Should().NotThrow();
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("Test Job Name")]
        [InlineData("Test-Job_Name")]
        [InlineData("Test123Job")]
        [InlineData("Test_Job-Name123")]
        public async Task BetLimit_WithVariousJobNames_ShouldCallService(string jobName)
        {
            // Arrange
            customerBetLimitJobService.Execute().Returns(Task.FromResult(true));

            // Act
            var result = await controller.BetLimit(jobName);

            // Assert
            result.Should().BeTrue();
            customerBetLimitJobService.Received(1).Execute();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task SapaSportDangerLevel_WithVariousBatchSizes_ShouldCallService(int batchSize)
        {
            // Arrange
            sapaSportDangerLevelJobService.Execute(batchSize).Returns(Task.FromResult(true));

            // Act
            var result = await controller.SapaSportDangerLevel(batchSize);

            // Assert
            result.Should().BeTrue();
            sapaSportDangerLevelJobService.Received(1).Execute(batchSize);
        }

        #endregion

        #region Controller Attributes Tests

        [Fact]
        public void JobController_ShouldHaveApiControllerAttribute()
        {
            // Act & Assert
            var attributes = typeof(JobController).GetCustomAttributes(typeof(ApiControllerAttribute), false);
            attributes.Should().HaveCount(1);
        }

        [Fact]
        public void JobController_ShouldHaveRouteAttribute()
        {
            // Act & Assert
            var attributes = typeof(JobController).GetCustomAttributes(typeof(RouteAttribute), false);
            attributes.Should().HaveCount(1);
            
            var routeAttribute = attributes[0] as RouteAttribute;
            routeAttribute.Template.Should().Be("api/job");
        }

        [Fact]
        public void BetLimit_ShouldHaveHttpPostAttribute()
        {
            // Act & Assert
            var method = typeof(JobController).GetMethod("BetLimit");
            var attributes = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
            attributes.Should().HaveCount(1);
            
            var httpPostAttribute = attributes[0] as HttpPostAttribute;
            httpPostAttribute.Template.Should().Be("betlimit");
        }

        [Fact]
        public void MemberBigStake_ShouldHaveHttpPostAttribute()
        {
            // Act & Assert
            var method = typeof(JobController).GetMethod("MemberBigStake");
            var attributes = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
            attributes.Should().HaveCount(1);
            
            var httpPostAttribute = attributes[0] as HttpPostAttribute;
            httpPostAttribute.Template.Should().Be("memberbigstake");
        }

        [Fact]
        public void SpecialLicensee_ShouldHaveHttpPostAttribute()
        {
            // Act & Assert
            var method = typeof(JobController).GetMethod("SpecialLicensee");
            var attributes = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
            attributes.Should().HaveCount(1);
            
            var httpPostAttribute = attributes[0] as HttpPostAttribute;
            httpPostAttribute.Template.Should().Be("spaciallicensee");
        }

        [Fact]
        public void Agent_ShouldHaveHttpPostAttribute()
        {
            // Act & Assert
            var method = typeof(JobController).GetMethod("Agent");
            var attributes = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
            attributes.Should().HaveCount(1);
            
            var httpPostAttribute = attributes[0] as HttpPostAttribute;
            httpPostAttribute.Template.Should().Be("agent");
        }

        [Fact]
        public void AdjustVRSetting_ShouldHaveHttpPostAttribute()
        {
            // Act & Assert
            var method = typeof(JobController).GetMethod("AdjustVRSetting");
            var attributes = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
            attributes.Should().HaveCount(1);
            
            var httpPostAttribute = attributes[0] as HttpPostAttribute;
            httpPostAttribute.Template.Should().Be("adjustvrsetting");
        }

        [Fact]
        public void WriteLog_ShouldHaveHttpPostAttribute()
        {
            // Act & Assert
            var method = typeof(JobController).GetMethod("WriteLog");
            var attributes = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
            attributes.Should().HaveCount(1);
            
            var httpPostAttribute = attributes[0] as HttpPostAttribute;
            httpPostAttribute.Template.Should().Be("WriteLog");
        }

        [Fact]
        public void SapaSportDangerLevel_ShouldHaveHttpPostAttribute()
        {
            // Act & Assert
            var method = typeof(JobController).GetMethod("SapaSportDangerLevel");
            var attributes = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
            attributes.Should().HaveCount(1);
            
            var httpPostAttribute = attributes[0] as HttpPostAttribute;
            httpPostAttribute.Template.Should().Be("SapaSportDangerLevel");
        }

        [Fact]
        public void SapaSportDangerLevel_ShouldHaveFromFormAttribute()
        {
            // Act & Assert
            var method = typeof(JobController).GetMethod("SapaSportDangerLevel");
            var parameters = method.GetParameters();
            parameters.Should().HaveCount(1);
            
            var parameter = parameters[0];
            var attributes = parameter.GetCustomAttributes(typeof(FromFormAttribute), false);
            attributes.Should().HaveCount(1);
        }

        #endregion
    }
}
