namespace Fanex.NAP.SPU.VR.Tests.Features.MemberBigStake
{
    using Fanex.NAP.SPU.VR.Features.MemberBigStake;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using FluentAssertions;
    using System;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Unit tests for MemberBigStakeDataAccess class
    /// </summary>
    public class MemberBigStakeDataAccessTest
    {
        private readonly MemberBigStakeDataAccess dataAccess;

        public MemberBigStakeDataAccessTest()
        {
            dataAccess = new MemberBigStakeDataAccess();
        }      

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var dataAccess = new MemberBigStakeDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
        }

        #endregion

        #region Method Signature Tests

        [Fact]
        public void GetMemberBigStakePerformance_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(MemberBigStakeDataAccess).GetMethod("GetMemberBigStakePerformance");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(MemberBigStakePerformanceModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(bool));
        }

        [Fact]
        public void InsertMemberBigStake_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(MemberBigStakeDataAccess).GetMethod("InsertMemberBigStake");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(bool));
        }

        [Fact]
        public void UpdateMemeberBigStakeFirstBetDate_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(MemberBigStakeDataAccess).GetMethod("UpdateMemeberBigStakeFirstBetDate");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(4);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(int));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(DateTime));
            method.GetParameters()[3].ParameterType.Should().Be(typeof(bool));
        }

        [Fact]
        public void UpdateWhitelistMemberBigStake_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(MemberBigStakeDataAccess).GetMethod("UpdateWhitelistMemberBigStake");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(3);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(int));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(bool));
        }

        [Fact]
        public void GetBigCreditMemberForAutomail_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(MemberBigStakeDataAccess).GetMethod("GetBigCreditMemberForAutomail");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(AutoMailBigCreditMemberModel));
            method.GetParameters().Should().BeEmpty();
        }

        #endregion

        #region Interface Implementation Tests

        [Fact]
        public void ShouldImplementIMemberBigStakeDataAccess()
        {
            // Assert
            dataAccess.Should().BeAssignableTo<IMemberBigStakeDataAccess>();
        }

        [Fact]
        public void ShouldHaveAllInterfaceMethods()
        {
            // Arrange
            var interfaceType = typeof(IMemberBigStakeDataAccess);
            var implementationType = typeof(MemberBigStakeDataAccess);

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