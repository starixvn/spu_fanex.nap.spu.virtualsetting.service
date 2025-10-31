namespace Fanex.NAP.SPU.VR.Tests.Features.Agent
{
    using Fanex.NAP.SPU.VR.Features.Agent;
    using Fanex.NAP.SPU.VR.Features.Agent.Entities;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Unit tests for AgentService class
    /// </summary>
    public class AgentServiceTest
    {
        private readonly IAgentDataAccess dataAccess;
        private readonly IAgentService service;

        public AgentServiceTest()
        {
            dataAccess = Substitute.For<IAgentDataAccess>();
            service = new AgentService(dataAccess);
        }

        #region InsertNewAgentAdjust Tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InsertNewAgentAdjust_WhenCalled_ShouldCallDataAccessWithCorrectParameters(bool isTest)
        {
            // Act
            service.InsertNewAgentAdjust(isTest);

            // Assert
            dataAccess.Received(1).InsertNewAgentAdjust(isTest);
        }

        #endregion

        #region VerifySportsAgentPerformance Tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void VerifySportsAgentPerformance_WhenCalled_ShouldReturnExpectedResult(bool isTest)
        {
            // Arrange
            var expectedResult = AgentExpectedData.ExpectedVerifySportsAgentPerformance;
            dataAccess.VerifySportsAgentPerformance(isTest).Returns(expectedResult);

            // Act
            var result = service.VerifySportsAgentPerformance(isTest);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            dataAccess.Received(1).VerifySportsAgentPerformance(isTest);
        }

        [Fact]
        public void VerifySportsAgentPerformance_WhenDataAccessReturnsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var emptyResult = new List<UnflagAbnormalAgentEntity>();
            dataAccess.VerifySportsAgentPerformance(Arg.Any<bool>()).Returns(emptyResult);

            // Act
            var result = service.VerifySportsAgentPerformance(false);

            // Assert
            result.Should().BeEmpty();
            dataAccess.Received(1).VerifySportsAgentPerformance(false);
        }

        #endregion

        #region UpdateFlagAbnormalAgent Tests

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(0, true)]
        [InlineData(-1, false)]
        public void UpdateFlagAbnormalAgent_WhenCalled_ShouldCallDataAccessWithCorrectParameters(int type, bool isTest)
        {
            // Act
            service.UpdateFlagAbnormalAgent(type, isTest);

            // Assert
            dataAccess.Received(1).UpdateFlagAbnormalAgent(type, isTest);
        }

        #endregion

        #region UpdateUnflagAbnormalAgent Tests

        [Theory]
        [InlineData("788945, 456642, 43352", 1, true)]
        [InlineData("123456", 2, false)]
        [InlineData("single_customer", 3, true)]
        public void UpdateUnflagAbnormalAgent_WhenValidCustomerList_ShouldCallDataAccessWithCorrectParameters(string customerList, int type, bool isTest)
        {
            // Act
            service.UpdateUnflagAbnormalAgent(customerList, type, isTest);

            // Assert
            dataAccess.Received(1).UpdateUnflagAbnormalAgent(customerList, type, isTest);
        }

        [Theory]
        [InlineData(null, 1, true)]
        [InlineData("", 2, false)]      
        public void UpdateUnflagAbnormalAgent_WhenInvalidCustomerList_ShouldNotCallDataAccess(string customerList, int type, bool isTest)
        {
            // Act
            service.UpdateUnflagAbnormalAgent(customerList, type, isTest);

            // Assert
            dataAccess.DidNotReceive().UpdateUnflagAbnormalAgent(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<bool>());
        }

        [Fact]
        public void UpdateUnflagAbnormalAgent_WhenCustomerListIsWhitespace_ShouldCallDataAccess()
        {
            // Arrange
            var whitespaceCustomerList = "   \t\n   ";

            // Act
            service.UpdateUnflagAbnormalAgent(whitespaceCustomerList, 1, false);

            // Assert
            dataAccess.Received(1).UpdateUnflagAbnormalAgent(whitespaceCustomerList, 1, false);
        }

        #endregion

        #region GetVRAgentUpdated Tests

        [Fact]
        public void GetVRAgentUpdated_WhenCalled_ShouldReturnExpectedResult()
        {
            // Arrange
            var expectedResult = AgentExpectedData.ExpectedGetVRAgentUpdated;
            dataAccess.GetVRAgentUpdated().Returns(expectedResult);

            // Act
            var result = service.GetVRAgentUpdated();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            dataAccess.Received(1).GetVRAgentUpdated();
        }

        [Fact]
        public void GetVRAgentUpdated_WhenDataAccessReturnsEmptyTuples_ShouldReturnEmptyCollections()
        {
            // Arrange
            var emptyResult = (new List<VRAgentUpdatedToZeroModel>(), new List<VRAgentUpdatedToFollowSuperModel>());
            dataAccess.GetVRAgentUpdated().Returns(emptyResult);

            // Act
            var result = service.GetVRAgentUpdated();

            // Assert
            result.Item1.Should().BeEmpty();
            result.Item2.Should().BeEmpty();
            dataAccess.Received(1).GetVRAgentUpdated();
        }

        [Fact]
        public void GetVRAgentUpdated_WhenDataAccessReturnsPartialData_ShouldReturnCorrectData()
        {
            // Arrange
            var zeroModels = new List<VRAgentUpdatedToZeroModel>
            {
                new VRAgentUpdatedToZeroModel { Agent = "agent01", Site = "12Bet" }
            };
            var followSuperModels = new List<VRAgentUpdatedToFollowSuperModel>();
            var expectedResult = (zeroModels, followSuperModels);
            dataAccess.GetVRAgentUpdated().Returns(expectedResult);

            // Act
            var result = service.GetVRAgentUpdated();

            // Assert
            result.Item1.Should().HaveCount(1);
            result.Item1.Should().BeEquivalentTo(zeroModels);
            result.Item2.Should().BeEmpty();
            dataAccess.Received(1).GetVRAgentUpdated();
        }

        #endregion
    }
}
