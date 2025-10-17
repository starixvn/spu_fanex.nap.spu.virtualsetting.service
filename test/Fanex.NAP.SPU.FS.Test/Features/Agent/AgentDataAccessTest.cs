namespace Fanex.NAP.SPU.VR.Tests.Features.Agent
{
    using Fanex.NAP.SPU.VR.Features.Agent;
    using Fanex.NAP.SPU.VR.Features.Agent.Entities;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Unit tests for AgentDataAccess class
    /// </summary>
    public class AgentDataAccessTest
    {
        private readonly AgentDataAccess dataAccess;

        public AgentDataAccessTest()
        {
            dataAccess = new AgentDataAccess();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var dataAccess = new AgentDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
        }

        #endregion                

        #region Method Signature Tests

        [Fact]
        public void InsertNewAgentAdjust_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(AgentDataAccess).GetMethod("InsertNewAgentAdjust");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(bool));
        }

        [Fact]
        public void VerifySportsAgentPerformance_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(AgentDataAccess).GetMethod("VerifySportsAgentPerformance");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(IEnumerable<UnflagAbnormalAgentEntity>));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(bool));
        }

        [Fact]
        public void UpdateFlagAbnormalAgent_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(AgentDataAccess).GetMethod("UpdateFlagAbnormalAgent");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(2);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(int));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(bool));
        }

        [Fact]
        public void UpdateUnflagAbnormalAgent_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(AgentDataAccess).GetMethod("UpdateUnflagAbnormalAgent");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(3);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(int));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(bool));
        }

        [Fact]
        public void GetVRAgentUpdated_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(AgentDataAccess).GetMethod("GetVRAgentUpdated");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof((IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>)));
            method.GetParameters().Should().BeEmpty();
        }

        #endregion

        #region Interface Implementation Tests

        [Fact]
        public void ShouldImplementIAgentDataAccess()
        {
            // Assert
            dataAccess.Should().BeAssignableTo<IAgentDataAccess>();
        }

        [Fact]
        public void ShouldHaveAllInterfaceMethods()
        {
            // Arrange
            var interfaceType = typeof(IAgentDataAccess);
            var implementationType = typeof(AgentDataAccess);

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
       
    }
}