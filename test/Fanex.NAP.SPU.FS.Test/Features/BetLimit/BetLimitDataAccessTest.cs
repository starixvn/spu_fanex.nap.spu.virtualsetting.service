namespace Fanex.NAP.SPU.VR.Tests.Features.BetLimit
{
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Unit tests for BetLimitDataAccess class
    /// </summary>
    public class BetLimitDataAccessTest
    {
        private readonly BetLimitDataAccess dataAccess;

        public BetLimitDataAccessTest()
        {
            dataAccess = new BetLimitDataAccess();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var dataAccess = new BetLimitDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
        }

        #endregion      
   
        #region Method Signature Tests

        [Fact]
        public void GetCustomersBetLimit_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(BetLimitDataAccess).GetMethod("GetCustomersBetLimit");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(IEnumerable<int>));
            method.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void RemoveBetLimit_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(BetLimitDataAccess).GetMethod("RemoveBetLimit");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void UpdateBetLimit_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(BetLimitDataAccess).GetMethod("UpdateBetLimit");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void VerifyCustomersBetLimit_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(BetLimitDataAccess).GetMethod("VerifyCustomersBetLimit");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof((IEnumerable<int>, IEnumerable<int>)));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        #endregion

        #region Interface Implementation Tests

        [Fact]
        public void ShouldImplementIBetLimitDataAccess()
        {
            // Assert
            dataAccess.Should().BeAssignableTo<IBetLimitDataAccess>();
        }

        [Fact]
        public void ShouldHaveAllInterfaceMethods()
        {
            // Arrange
            var interfaceType = typeof(IBetLimitDataAccess);
            var implementationType = typeof(BetLimitDataAccess);

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
       
        #region Return Type Tests

        [Fact]
        public void RemoveBetLimit_ShouldReturnVoid()
        {
            // Arrange
            var method = typeof(BetLimitDataAccess).GetMethod("RemoveBetLimit");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void UpdateBetLimit_ShouldReturnVoid()
        {
            // Arrange
            var method = typeof(BetLimitDataAccess).GetMethod("UpdateBetLimit");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
        }

        #endregion               
       
    }
}