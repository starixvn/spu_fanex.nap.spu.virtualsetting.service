namespace Fanex.NAP.SPU.VR.Tests.Features.Shares
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Agent;
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake;
    using Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Unit tests for InitService class
    /// </summary>
    public class InitServiceTest
    {
        #region RegisterService Tests

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterAllServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            services.Should().NotBeEmpty();
            services.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterBetLimitServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IBetLimitDataAccess));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IBetLimitService));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(ICustomerBetLimitJobService));
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterAdjust3MServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IAdjust3MDataAccess));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(ISoccerService));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(ISportsbookService));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IAdjust3MJobService));
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterAgentServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IAgentDataAccess));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IAgentService));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IAgentJobService));
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterMemberBigStakeServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IMemberBigStakeDataAccess));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IMemberBigStakeService));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IMemberBigStakeJobService));
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterSpecialLicenseeServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(ISpecialLicenseeDataAccess));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(ISpecialLicenseeService));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(ISpecialLicenseeJobService));
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterSapaSportDangerLevelServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IDangerLevelSapaSportDataAccess));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(IDangerLevelSapaSportService));
            serviceDescriptors.Should().Contain(sd => sd.ServiceType == typeof(ISapaSportDangerLevelJobService));
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterAllServicesAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            serviceDescriptors.Should().OnlyContain(sd => sd.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterCorrectImplementationTypes()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            
            // BetLimit services
            var betLimitDataAccess = serviceDescriptors.First(sd => sd.ServiceType == typeof(IBetLimitDataAccess));
            betLimitDataAccess.ImplementationType.Should().Be(typeof(BetLimitDataAccess));
            
            var betLimitService = serviceDescriptors.First(sd => sd.ServiceType == typeof(IBetLimitService));
            betLimitService.ImplementationType.Should().Be(typeof(BetLimitService));
            
            var customerBetLimitJobService = serviceDescriptors.First(sd => sd.ServiceType == typeof(ICustomerBetLimitJobService));
            customerBetLimitJobService.ImplementationType.Should().Be(typeof(CustomerBetLimitJobService));
        }

        #endregion

        #region Service Registration Order Tests

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterServicesInCorrectOrder()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            
            // Verify that BetLimit services are registered first
            var betLimitDataAccessIndex = serviceDescriptors.FindIndex(sd => sd.ServiceType == typeof(IBetLimitDataAccess));
            var adjust3MDataAccessIndex = serviceDescriptors.FindIndex(sd => sd.ServiceType == typeof(IAdjust3MDataAccess));
            
            betLimitDataAccessIndex.Should().BeLessThan(adjust3MDataAccessIndex);
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterDataAccessBeforeServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            
            // For each feature, data access should be registered before the service
            var betLimitDataAccessIndex = serviceDescriptors.FindIndex(sd => sd.ServiceType == typeof(IBetLimitDataAccess));
            var betLimitServiceIndex = serviceDescriptors.FindIndex(sd => sd.ServiceType == typeof(IBetLimitService));
            
            betLimitDataAccessIndex.Should().BeLessThan(betLimitServiceIndex);
        }

        #endregion

        #region Service Resolution Tests       

        [Fact]
        public void RegisterService_WhenCalled_ShouldResolveAllServicesAsSingletons()
        {
            // Arrange
            var services = new ServiceCollection();
            services.RegisterService();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var betLimitService1 = serviceProvider.GetService<IBetLimitService>();
            var betLimitService2 = serviceProvider.GetService<IBetLimitService>();

            // Assert
            betLimitService1.Should().BeSameAs(betLimitService2);
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public void RegisterService_WhenCalledWithNullServiceCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            IServiceCollection nullServices = null;

            // Act & Assert
            var action = () => nullServices.RegisterService();
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RegisterService_WhenCalledMultipleTimes_ShouldNotThrowException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            var action = () =>
            {
                services.RegisterService();
                services.RegisterService();
            };
            action.Should().NotThrow();
        }
      
        [Fact]
        public void RegisterService_WhenCalled_ShouldNotRegisterDuplicateServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceDescriptors = services.ToList();
            var duplicateServices = serviceDescriptors
                .GroupBy(sd => sd.ServiceType)
                .Where(g => g.Count() > 1)
                .ToList();

            duplicateServices.Should().BeEmpty();
        }

        #endregion
       
    }
}
