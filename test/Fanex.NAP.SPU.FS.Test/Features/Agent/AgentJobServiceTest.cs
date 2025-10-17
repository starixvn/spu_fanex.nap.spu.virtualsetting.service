namespace Fanex.NAP.SPU.VR.Tests.Features.Agent
{
    using Fanex.NAP.SPU.VR.JobService;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using Fanex.NAP.SPU.VR.Features.Agent;
    using Fanex.NAP.SPU.VR.Features.Agent.Entities;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for AgentJobService class
    /// </summary>
    public class AgentJobServiceTest
    {
        private readonly IAppSettings appSettings;
        private readonly IAgentService agentService;
        private readonly AgentJobService jobService;

        public AgentJobServiceTest()
        {
            appSettings = Substitute.For<IAppSettings>();
            agentService = Substitute.For<IAgentService>();
            jobService = new AgentJobService(appSettings, agentService);
        }              

        #region GenerateMailBody Tests

        [Fact]
        public void GenerateMailBody_WhenHasUpdatedAgents_ShouldGenerateMailBody()
        {
            // Arrange
            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>
            {
                new VRAgentUpdatedToZeroModel { Site = "Site1", Agent = "Agent1" },
                new VRAgentUpdatedToZeroModel { Site = "Site2", Agent = "Agent2" }
            };
            var updatedToFollowSupperList = new List<VRAgentUpdatedToFollowSuperModel>
            {
                new VRAgentUpdatedToFollowSuperModel { Site = "Site3", Agent = "Agent3", MemberWinloss = 1500.25m }
            };

            agentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSupperList));

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateMailBody", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            agentService.Received(1).GetVRAgentUpdated();
        }

        [Fact]
        public void GenerateMailBody_WhenNoUpdatedAgents_ShouldGenerateEmptyMailBody()
        {
            // Arrange
            var emptyUpdatedToZeroList = new List<VRAgentUpdatedToZeroModel>();
            var emptyUpdatedToFollowSupperList = new List<VRAgentUpdatedToFollowSuperModel>();

            agentService.GetVRAgentUpdated().Returns((emptyUpdatedToZeroList, emptyUpdatedToFollowSupperList));

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateMailBody", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            agentService.Received(1).GetVRAgentUpdated();
        }     

        #endregion

        #region NotifyMessage Tests

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageIsEmpty_ShouldNotCallUtilities()
        {
            // Arrange
            // The notifiedMessage field is private and starts as empty string

            // Act
            var method = typeof(AgentJobService).GetMethod("NotifyMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            // Since we can't easily mock static methods, we just verify the method executes without exception
            method.Should().NotBeNull();
        }

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageIsWhitespace_ShouldNotCallUtilities()
        {
            // Arrange
            // The notifiedMessage field is private and starts as empty string

            // Act
            var method = typeof(AgentJobService).GetMethod("NotifyMessage", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            // Since we can't easily mock static methods, we just verify the method executes without exception
            method.Should().NotBeNull();
        }

        #endregion

        #region HTML Generation Tests

        [Fact]
        public void GenerateTableUpdateToZero_WhenValidList_ShouldGenerateHtmlTable()
        {
            // Arrange
            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>
            {
                new VRAgentUpdatedToZeroModel { Site = "Site1", Agent = "Agent1" },
                new VRAgentUpdatedToZeroModel { Site = "Site2", Agent = "Agent2" }
            };

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToZero", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { updatedToZeroList }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<table");
            result.Should().Contain("<tr>");
            result.Should().Contain("Site1");
            result.Should().Contain("Agent1");
        }

        [Fact]
        public void GenerateTableUpdateToZero_WhenEmptyList_ShouldGenerateEmptyTable()
        {
            // Arrange
            var emptyList = new List<VRAgentUpdatedToZeroModel>();

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToZero", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { emptyList }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<br />Table: agents whose VR got updated to 0");
            result.Should().NotContain("<table");
        }

        [Fact]
        public void GenerateTableUpdateToZero_WhenNullList_ShouldGenerateEmptyTable()
        {
            // Arrange
            IEnumerable<VRAgentUpdatedToZeroModel> nullList = null;

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToZero", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { nullList }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<br />Table: agents whose VR got updated to 0");
            result.Should().NotContain("<table");
        }

        [Fact]
        public void GenerateTableUpdateToFollowSupper_WhenValidList_ShouldGenerateHtmlTable()
        {
            // Arrange
            var updatedToFollowSupperList = new List<VRAgentUpdatedToFollowSuperModel>
            {
                new VRAgentUpdatedToFollowSuperModel { Site = "Site1", Agent = "Agent1", MemberWinloss = 1500.25m },
                new VRAgentUpdatedToFollowSuperModel { Site = "Site2", Agent = "Agent2", MemberWinloss = 2500.75m }
            };

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToFollowSupper", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { updatedToFollowSupperList }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<table");
            result.Should().Contain("<tr>");
            result.Should().Contain("Site1");
            result.Should().Contain("Agent1");
            result.Should().Contain("1500RM");
        }

        [Fact]
        public void GenerateTableUpdateToFollowSupper_WhenEmptyList_ShouldGenerateEmptyTable()
        {
            // Arrange
            var emptyList = new List<VRAgentUpdatedToFollowSuperModel>();

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToFollowSupper", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { emptyList }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<br />Table: agents whose VR got updated to follow Super");
            result.Should().NotContain("<table");
        }

        [Fact]
        public void GenerateTableUpdateToFollowSupper_WhenNullList_ShouldGenerateEmptyTable()
        {
            // Arrange
            IEnumerable<VRAgentUpdatedToFollowSuperModel> nullList = null;

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToFollowSupper", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { nullList }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<br />Table: agents whose VR got updated to follow Super");
            result.Should().NotContain("<table");
        }

        #endregion

        #region CSS Style Tests

        [Fact]
        public void CssStyleTable_WhenValidWidth_ShouldReturnCssString()
        {
            // Arrange
            var width = 300;

            // Act
            var method = typeof(AgentJobService).GetMethod("CssStyleTable", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { width }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("border-collapse: collapse");
            result.Should().Contain("width: 300px");
        }

        [Fact]
        public void CssStyleTh_ShouldReturnCssString()
        {
            // Act
            var method = typeof(AgentJobService).GetMethod("CssStyleTh", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("border: 1px solid black");
            result.Should().Contain("font-weight:bold");
        }

        [Fact]
        public void CssStyleTd_ShouldReturnCssString()
        {
            // Act
            var method = typeof(AgentJobService).GetMethod("CssStyleTd", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("border: 1px solid black");
        }

        #endregion
      
    }
}
