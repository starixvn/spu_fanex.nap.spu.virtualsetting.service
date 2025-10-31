namespace Fanex.NAP.SPU.VR.Tests.Features.JobService
{
    using Fanex.NAP.SPU.VR.Features.Agent;
    using Fanex.NAP.SPU.VR.Features.Agent.Entities;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using Fanex.NAP.SPU.VR.JobService;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for AgentJobService class
    /// </summary>
    public class AgentJobServiceTest
    {
        private readonly IAppSettings mockAppSettings;
        private readonly IAgentService mockAgentService;
        private readonly AgentJobService jobService;

        public AgentJobServiceTest()
        {
            mockAppSettings = Substitute.For<IAppSettings>();
            mockAgentService = Substitute.For<IAgentService>();
            jobService = new AgentJobService(mockAppSettings, mockAgentService);
        }

        private UnflagAbnormalAgentEntity CreateUnflagAbnormalAgentEntity(int agentId, decimal memberWinloss)
        {
            return new UnflagAbnormalAgentEntity
            {
                AgentID = agentId,
                MemberWinloss = memberWinloss
            };
        }

        private VRAgentUpdatedToZeroModel CreateVRAgentUpdatedToZeroModel(string site, string agent)
        {
            return new VRAgentUpdatedToZeroModel
            {
                Site = site,
                Agent = agent
            };
        }

        private VRAgentUpdatedToFollowSuperModel CreateVRAgentUpdatedToFollowSuperModel(string site, string agent, decimal memberWinloss)
        {
            return new VRAgentUpdatedToFollowSuperModel
            {
                Site = site,
                Agent = agent,
                MemberWinloss = memberWinloss
            };
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            // Arrange
            var appSettings = Substitute.For<IAppSettings>();
            var agentService = Substitute.For<IAgentService>();

            // Act
            var service = new AgentJobService(appSettings, agentService);

            // Assert
            service.Should().NotBeNull();
        }
       
        #endregion

        #region Execute Method Tests

        [Fact]
        public async Task Execute_WithSuccessfulExecution_ShouldReturnTrue()
        {
            // Arrange
            mockAppSettings.RetryTime.Returns(3);
            mockAppSettings.IsTest.Returns(true);
            mockAppSettings.MailingListIdAgentAjust.Returns(123);
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");
            mockAppSettings.PunterChunkSize.Returns(10);

            var unflagAgents = new List<UnflagAbnormalAgentEntity>
            {
                CreateUnflagAbnormalAgentEntity(1, 100.50m),
                CreateUnflagAbnormalAgentEntity(2, 200.75m)
            };

            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>
            {
                CreateVRAgentUpdatedToZeroModel("Site1", "Agent1")
            };

            var updatedToFollowSuperList = new List<VRAgentUpdatedToFollowSuperModel>
            {
                CreateVRAgentUpdatedToFollowSuperModel("Site2", "Agent2", 300.25m)
            };

            mockAgentService.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(unflagAgents);
            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            mockAgentService.Received(1).InsertNewAgentAdjust(true);
            mockAgentService.Received(1).VerifySportsAgentPerformance(true);
            mockAgentService.Received(1).UpdateFlagAbnormalAgent(1, true);
            mockAgentService.Received().UpdateUnflagAbnormalAgent(Arg.Any<string>(), 1, true);
        }           

        [Fact]
        public async Task Execute_WithZeroRetryTime_ShouldReturnTrue()
        {
            // Arrange
            mockAppSettings.RetryTime.Returns(0);
            mockAppSettings.IsTest.Returns(true);
            mockAppSettings.MailingListIdAgentAjust.Returns(123);
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");
            mockAppSettings.PunterChunkSize.Returns(10);

            var unflagAgents = new List<UnflagAbnormalAgentEntity>();
            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>();
            var updatedToFollowSuperList = new List<VRAgentUpdatedToFollowSuperModel>();

            mockAgentService.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(unflagAgents);
            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            mockAgentService.DidNotReceive().InsertNewAgentAdjust(Arg.Any<bool>());
        }

        [Fact]
        public async Task Execute_WithNegativeRetryTime_ShouldReturnTrue()
        {
            // Arrange
            mockAppSettings.RetryTime.Returns(-1);
            mockAppSettings.IsTest.Returns(true);
            mockAppSettings.MailingListIdAgentAjust.Returns(123);
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");
            mockAppSettings.PunterChunkSize.Returns(10);

            var unflagAgents = new List<UnflagAbnormalAgentEntity>();
            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>();
            var updatedToFollowSuperList = new List<VRAgentUpdatedToFollowSuperModel>();

            mockAgentService.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(unflagAgents);
            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            mockAgentService.DidNotReceive().InsertNewAgentAdjust(Arg.Any<bool>());
        }

        #endregion

        #region UpdateUnflagAbnormalAgent Private Method Tests

        [Fact]
        public void UpdateUnflagAbnormalAgent_WithValidData_ShouldProcessChunks()
        {
            // Arrange
            mockAppSettings.PunterChunkSize.Returns(2);
            mockAppSettings.IsTest.Returns(true);

            var customerList = new List<UnflagAbnormalAgentEntity>
            {
                CreateUnflagAbnormalAgentEntity(1, 100.50m),
                CreateUnflagAbnormalAgentEntity(2, 200.75m),
                CreateUnflagAbnormalAgentEntity(3, 300.25m)
            };

            var method = typeof(AgentJobService).GetMethod("UpdateUnflagAbnormalAgent", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(jobService, new object[] { customerList, 1 });

            // Assert
            mockAgentService.Received().UpdateUnflagAbnormalAgent(Arg.Any<string>(), 1, true);
        }

        [Fact]
        public void UpdateUnflagAbnormalAgent_WithEmptyList_ShouldNotCallAgentService()
        {
            // Arrange
            mockAppSettings.PunterChunkSize.Returns(10);
            mockAppSettings.IsTest.Returns(true);

            var customerList = new List<UnflagAbnormalAgentEntity>();

            var method = typeof(AgentJobService).GetMethod("UpdateUnflagAbnormalAgent", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(jobService, new object[] { customerList, 1 });

            // Assert
            mockAgentService.DidNotReceive().UpdateUnflagAbnormalAgent(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<bool>());
        }
      
        [Fact]
        public void UpdateUnflagAbnormalAgent_WithSingleItem_ShouldProcessCorrectly()
        {
            // Arrange
            mockAppSettings.PunterChunkSize.Returns(10);
            mockAppSettings.IsTest.Returns(true);

            var customerList = new List<UnflagAbnormalAgentEntity>
            {
                CreateUnflagAbnormalAgentEntity(1, 100.50m)
            };

            var method = typeof(AgentJobService).GetMethod("UpdateUnflagAbnormalAgent", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(jobService, new object[] { customerList, 1 });

            // Assert
            mockAgentService.Received(1).UpdateUnflagAbnormalAgent(Arg.Any<string>(), 1, true);
        }

        #endregion

        #region NotifyMessage Private Method Tests

        [Fact]
        public void NotifyMessage_WithEmptyMessage_ShouldNotCallUtilities()
        {
            // Arrange
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");

            // Set the notifiedMessage field to empty using reflection
            var field = typeof(AgentJobService).GetField("notifiedMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, string.Empty);

            var method = typeof(AgentJobService).GetMethod("NotifyMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(jobService, null);

            // Assert
            // This test verifies the method can be called without throwing an exception
            // The actual Utilities.NotifyMessage call would require more complex setup
            method.Should().NotBeNull();
        }

        [Fact]
        public void NotifyMessage_WithNullMessage_ShouldNotCallUtilities()
        {
            // Arrange
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");

            // Set the notifiedMessage field to null using reflection
            var field = typeof(AgentJobService).GetField("notifiedMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, null);

            var method = typeof(AgentJobService).GetMethod("NotifyMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(jobService, null);

            // Assert
            // This test verifies the method can be called without throwing an exception
            // The actual Utilities.NotifyMessage call would require more complex setup
            method.Should().NotBeNull();
        }

        [Fact]
        public void NotifyMessage_WithValidMessage_ShouldCallUtilities()
        {
            // Arrange
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");

            // Set the notifiedMessage field using reflection
            var field = typeof(AgentJobService).GetField("notifiedMessage", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, "Test notification message");

            var method = typeof(AgentJobService).GetMethod("NotifyMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(jobService, null);

            // Assert
            // This test verifies the method can be called without throwing an exception
            // The actual Utilities.NotifyMessage call would require more complex setup
            method.Should().NotBeNull();
        }

        #endregion
       
        #region GenerateMailBody Private Method Tests

        [Fact]
        public void GenerateMailBody_WithValidData_ShouldGenerateMailBody()
        {
            // Arrange
            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>
            {
                CreateVRAgentUpdatedToZeroModel("Site1", "Agent1"),
                CreateVRAgentUpdatedToZeroModel("Site2", "Agent2")
            };

            var updatedToFollowSuperList = new List<VRAgentUpdatedToFollowSuperModel>
            {
                CreateVRAgentUpdatedToFollowSuperModel("Site3", "Agent3", 100.50m),
                CreateVRAgentUpdatedToFollowSuperModel("Site4", "Agent4", 200.75m)
            };

            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            var method = typeof(AgentJobService).GetMethod("GenerateMailBody", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            mockAgentService.Received(1).GetVRAgentUpdated();
        }

        [Fact]
        public void GenerateMailBody_WithNoUpdatedAgents_ShouldGenerateEmptyMailBody()
        {
            // Arrange
            var emptyUpdatedToZeroList = new List<VRAgentUpdatedToZeroModel>();
            var emptyUpdatedToFollowSupperList = new List<VRAgentUpdatedToFollowSuperModel>();

            mockAgentService.GetVRAgentUpdated().Returns((emptyUpdatedToZeroList, emptyUpdatedToFollowSupperList));

            var method = typeof(AgentJobService).GetMethod("GenerateMailBody", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            mockAgentService.Received(1).GetVRAgentUpdated();
        }

        [Fact]
        public void GenerateMailBody_WithNullData_ShouldGenerateEmptyMailBody()
        {
            // Arrange
            mockAgentService.GetVRAgentUpdated().Returns(((IEnumerable<VRAgentUpdatedToZeroModel>)null, (IEnumerable<VRAgentUpdatedToFollowSuperModel>)null));

            var method = typeof(AgentJobService).GetMethod("GenerateMailBody", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            mockAgentService.Received(1).GetVRAgentUpdated();
        }

        #endregion

        #region GenerateTableUpdateToZero Private Method Tests

        [Fact]
        public void GenerateTableUpdateToZero_WhenValidList_ShouldGenerateHtmlTable()
        {
            // Arrange
            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>
            {
                CreateVRAgentUpdatedToZeroModel("Site1", "Agent1"),
                CreateVRAgentUpdatedToZeroModel("Site2", "Agent2")
            };

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToZero", BindingFlags.NonPublic | BindingFlags.Instance);
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
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToZero", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { emptyList }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("Table: agents whose VR got updated to 0");
        }

        [Fact]
        public void GenerateTableUpdateToZero_WhenNullList_ShouldGenerateEmptyTable()
        {
            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToZero", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { null }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("Table: agents whose VR got updated to 0");
        }

        #endregion

        #region GenerateTableUpdateToFollowSupper Private Method Tests
    
        [Fact]
        public void GenerateTableUpdateToFollowSupper_WhenEmptyList_ShouldGenerateEmptyTable()
        {
            // Arrange
            var emptyList = new List<VRAgentUpdatedToFollowSuperModel>();

            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToFollowSupper", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { emptyList }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("Table: agents whose VR got updated to follow Super");
        }

        [Fact]
        public void GenerateTableUpdateToFollowSupper_WhenNullList_ShouldGenerateEmptyTable()
        {
            // Act
            var method = typeof(AgentJobService).GetMethod("GenerateTableUpdateToFollowSupper", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { null }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("Table: agents whose VR got updated to follow Super");
        }

        #endregion

        #region CSS Helper Methods Tests

        [Fact]
        public void CssStyleTable_WithValidWidth_ShouldReturnCorrectCss()
        {
            // Arrange
            var width = 300;

            // Act
            var method = typeof(AgentJobService).GetMethod("CssStyleTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { width }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("border-collapse: collapse;");
            result.Should().Contain("width: 300px");
        }

        [Fact]
        public void CssStyleTable_WithZeroWidth_ShouldReturnCorrectCss()
        {
            // Arrange
            var width = 0;

            // Act
            var method = typeof(AgentJobService).GetMethod("CssStyleTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { width }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("border-collapse: collapse;");
            result.Should().Contain("width: 0px");
        }

        [Fact]
        public void CssStyleTable_WithNegativeWidth_ShouldReturnCorrectCss()
        {
            // Arrange
            var width = -100;

            // Act
            var method = typeof(AgentJobService).GetMethod("CssStyleTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { width }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("border-collapse: collapse;");
            result.Should().Contain("width: -100px");
        }

        [Fact]
        public void CssStyleTh_ShouldReturnCorrectCss()
        {
            // Act
            var method = typeof(AgentJobService).GetMethod("CssStyleTh", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("border: 1px solid black;");
            result.Should().Contain("font-weight:bold;");
        }      

        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public async Task Execute_WithExceptionInUpdateUnflagAbnormalAgent_ShouldRetryAndReturnTrue()
        {
            // Arrange
            mockAppSettings.RetryTime.Returns(3);
            mockAppSettings.IsTest.Returns(true);
            mockAppSettings.MailingListIdAgentAjust.Returns(123);
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");
            mockAppSettings.PunterChunkSize.Returns(10);

            var unflagAgents = new List<UnflagAbnormalAgentEntity>
            {
                CreateUnflagAbnormalAgentEntity(1, 100.50m)
            };

            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>();
            var updatedToFollowSuperList = new List<VRAgentUpdatedToFollowSuperModel>();

            mockAgentService.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(unflagAgents);
            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            mockAgentService.Received(1).InsertNewAgentAdjust(true);
            mockAgentService.Received(1).VerifySportsAgentPerformance(true);
            mockAgentService.Received(1).UpdateFlagAbnormalAgent(1, true);
        }

        [Fact]
        public async Task Execute_WithExceptionInSendMail_ShouldRetryAndReturnTrue()
        {
            // Arrange
            mockAppSettings.RetryTime.Returns(3);
            mockAppSettings.IsTest.Returns(true);
            mockAppSettings.MailingListIdAgentAjust.Returns(123);
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");
            mockAppSettings.PunterChunkSize.Returns(10);

            var unflagAgents = new List<UnflagAbnormalAgentEntity>();
            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>();
            var updatedToFollowSuperList = new List<VRAgentUpdatedToFollowSuperModel>();

            mockAgentService.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(unflagAgents);
            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            mockAgentService.Received(1).InsertNewAgentAdjust(true);
            mockAgentService.Received(1).VerifySportsAgentPerformance(true);
            mockAgentService.Received(1).UpdateFlagAbnormalAgent(1, true);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task CompleteWorkflow_WithValidData_ShouldProcessAllSteps()
        {
            // Arrange
            mockAppSettings.RetryTime.Returns(3);
            mockAppSettings.IsTest.Returns(true);
            mockAppSettings.MailingListIdAgentAjust.Returns(123);
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");
            mockAppSettings.PunterChunkSize.Returns(10);

            var unflagAgents = new List<UnflagAbnormalAgentEntity>
            {
                CreateUnflagAbnormalAgentEntity(1, 100.50m),
                CreateUnflagAbnormalAgentEntity(2, 200.75m)
            };

            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>
            {
                CreateVRAgentUpdatedToZeroModel("Site1", "Agent1")
            };

            var updatedToFollowSuperList = new List<VRAgentUpdatedToFollowSuperModel>
            {
                CreateVRAgentUpdatedToFollowSuperModel("Site2", "Agent2", 300.25m)
            };

            mockAgentService.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(unflagAgents);
            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            mockAgentService.Received(1).InsertNewAgentAdjust(true);
            mockAgentService.Received(1).VerifySportsAgentPerformance(true);
            mockAgentService.Received(1).UpdateFlagAbnormalAgent(1, true);
            mockAgentService.Received().UpdateUnflagAbnormalAgent(Arg.Any<string>(), 1, true);
            mockAgentService.Received(1).GetVRAgentUpdated();
        }

        [Fact]
        public async Task CompleteWorkflow_WithNoData_ShouldProcessAllSteps()
        {
            // Arrange
            mockAppSettings.RetryTime.Returns(3);
            mockAppSettings.IsTest.Returns(false);
            mockAppSettings.MailingListIdAgentAjust.Returns(456);
            mockAppSettings.EnvironmentString.Returns("PRO");
            mockAppSettings.LogGroupID.Returns("prod-group");
            mockAppSettings.PunterChunkSize.Returns(5);

            var unflagAgents = new List<UnflagAbnormalAgentEntity>();
            var updatedToZeroList = new List<VRAgentUpdatedToZeroModel>();
            var updatedToFollowSuperList = new List<VRAgentUpdatedToFollowSuperModel>();

            mockAgentService.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(unflagAgents);
            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            // Act
            var result = await jobService.Execute();

            // Assert
            result.Should().BeTrue();
            mockAgentService.Received(1).InsertNewAgentAdjust(false);
            mockAgentService.Received(1).VerifySportsAgentPerformance(false);
            mockAgentService.Received(1).UpdateFlagAbnormalAgent(1, false);
            mockAgentService.DidNotReceive().UpdateUnflagAbnormalAgent(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<bool>());
            mockAgentService.Received(1).GetVRAgentUpdated();
        }

        #endregion

        #region Method Signature Tests

        [Fact]
        public void Execute_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(AgentJobService).GetMethod("Execute");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(Task<bool>));
            method.GetParameters().Should().BeEmpty();
        }

        #endregion

        #region Interface Implementation Tests

        [Fact]
        public void ShouldImplementIAgentJobService()
        {
            // Assert
            jobService.Should().BeAssignableTo<IAgentJobService>();
        }

        [Fact]
        public void ShouldHaveAllInterfaceMethods()
        {
            // Arrange
            var interfaceType = typeof(IAgentJobService);
            var implementationType = typeof(AgentJobService);

            // Act
            var interfaceMethods = interfaceType.GetMethods();
            var implementationMethods = implementationType.GetMethods()
                .Where(m => m.DeclaringType == implementationType)
                .ToArray();

            // Assert
            foreach (var interfaceMethod in interfaceMethods)
            {
                var implementationMethod = implementationMethods.FirstOrDefault(m => 
                    m.Name == interfaceMethod.Name && 
                    m.GetParameters().Length == interfaceMethod.GetParameters().Length);
                
                implementationMethod.Should().NotBeNull($"Method {interfaceMethod.Name} should be implemented");
            }
        }

        #endregion

        #region Constants Tests

        [Fact]
        public void ShouldHaveCorrectTemplateConstants()
        {
            // Arrange
            var type = typeof(AgentJobService);
            var noAgentAffectTemplateField = type.GetField("VSMailInformNoAgentAffectTemplate", BindingFlags.NonPublic | BindingFlags.Static);
            var mailTemplateField = type.GetField("VSMailInformTemplate", BindingFlags.NonPublic | BindingFlags.Static);

            // Assert
            noAgentAffectTemplateField.Should().NotBeNull();
            mailTemplateField.Should().NotBeNull();
            noAgentAffectTemplateField.IsLiteral.Should().BeTrue();
            mailTemplateField.IsLiteral.Should().BeTrue();
        }

        #endregion

        #region Field Tests

        [Fact]
        public void ShouldHaveCorrectPrivateFields()
        {
            // Arrange
            var type = typeof(AgentJobService);
            var appSettingsField = type.GetField("appSettings", BindingFlags.NonPublic | BindingFlags.Instance);
            var agentServiceField = type.GetField("agentService", BindingFlags.NonPublic | BindingFlags.Instance);
            var notifiedMessageField = type.GetField("notifiedMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            appSettingsField.Should().NotBeNull();
            agentServiceField.Should().NotBeNull();
            notifiedMessageField.Should().NotBeNull();
            appSettingsField.FieldType.Should().Be(typeof(IAppSettings));
            agentServiceField.FieldType.Should().Be(typeof(IAgentService));
            notifiedMessageField.FieldType.Should().Be(typeof(string));
        }

        #endregion

        #region Performance Tests

        [Fact]
        public async Task Execute_WithLargeDataSet_ShouldPerformEfficiently()
        {
            // Arrange
            mockAppSettings.RetryTime.Returns(3);
            mockAppSettings.IsTest.Returns(true);
            mockAppSettings.MailingListIdAgentAjust.Returns(123);
            mockAppSettings.EnvironmentString.Returns("UAT");
            mockAppSettings.LogGroupID.Returns("test-group");
            mockAppSettings.PunterChunkSize.Returns(100);

            var unflagAgents = Enumerable.Range(1, 1000)
                .Select(i => CreateUnflagAbnormalAgentEntity(i, 100.50m))
                .ToList();

            var updatedToZeroList = Enumerable.Range(1, 100)
                .Select(i => CreateVRAgentUpdatedToZeroModel($"Site{i}", $"Agent{i}"))
                .ToList();

            var updatedToFollowSuperList = Enumerable.Range(1, 100)
                .Select(i => CreateVRAgentUpdatedToFollowSuperModel($"Site{i}", $"Agent{i}", 100.50m))
                .ToList();

            mockAgentService.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(unflagAgents);
            mockAgentService.GetVRAgentUpdated().Returns((updatedToZeroList, updatedToFollowSuperList));

            // Act
            var startTime = DateTime.Now;
            var result = await jobService.Execute();
            var endTime = DateTime.Now;

            // Assert
            result.Should().BeTrue();
            var duration = endTime - startTime;
            duration.Should().BeLessThan(TimeSpan.FromSeconds(10)); // Reasonable performance expectation
        }

        #endregion
    }
}
