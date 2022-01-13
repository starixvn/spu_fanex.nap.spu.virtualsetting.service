namespace Fanex.NAP.SPU.VR.Tests.Features.Agent
{
    using Fanex.NAP.SPU.VR.Features.Agent;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class AgentServiceTest
    {
        private readonly IAgentDataAccess dataAccess;
        private readonly IAgentService service;

        public AgentServiceTest()
        {
            dataAccess = Substitute.For<IAgentDataAccess>();

            service = new AgentService(dataAccess);
        }

        [Fact]
        public void InsertNewAgentAdjust_WhenSuccess() 
        {
            service.InsertNewAgentAdjust(false);            
        }

        [Fact]
        public void VerifySportsAgentPerformance_WhenSuccess()
        {
            var expectedResult = AgentExpectedData.ExpectedVerifySportsAgentPerformance;

            dataAccess.VerifySportsAgentPerformance(false).Returns(expectedResult);

            var result = service.VerifySportsAgentPerformance(false);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(1)]
        public void UpdateFlagAbnormalAgent_WhenSuccess(int type)
        {
            service.UpdateFlagAbnormalAgent(type, false);
        }

        [Theory]
        [InlineData("788945, 456642, 43352", 1, false)]
        public void UpdateUnflagAbnormalAgent_WhenSuccess(string customerList, int type, bool isTest)
        {
            service.UpdateUnflagAbnormalAgent(customerList, type, isTest);
        }

        [Fact]
        public void GetVRAgentUpdated_WhenSuccess()
        {
            var expectedResult = AgentExpectedData.ExpectedGetVRAgentUpdated;

            dataAccess.GetVRAgentUpdated().Returns(expectedResult);

            var result = service.GetVRAgentUpdated();

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
