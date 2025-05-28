namespace Fanex.NAP.SPU.VR.Tests.Features.SpecialLicensee
{
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System.Collections.Generic;
    using Xunit;
    using Fanex.Logging;

    public class SpecialLicenseeServiceTest
    {
        private readonly ISpecialLicenseeDataAccess dataAccess;
        private readonly ISpecialLicenseeService service;

        public SpecialLicenseeServiceTest()
        {
            // Setup test logging          
            var logEngine = Substitute.For<ILogEngine>();

            LogManager
                .SetDefaultLogCategory("")
                .Use(logEngine);

            dataAccess = Substitute.For<ISpecialLicenseeDataAccess>();
            service = new SpecialLicenseeService(dataAccess);
        }

        [Theory]
        [InlineData(500, false)]
        public void AutoAdjustForBasketball_WhenSuccess(int punterChunkSize, bool isTest)
        {
            // Arrange
            var customerList = new List<int> { 1, 2, 3 };
            var passedCustomers = new List<PassedPerfCheckCustomerEntity>
            {
                new PassedPerfCheckCustomerEntity { CustID = 1, MemberWinloss = 100, MemberTotalTurnover = 1000, MemberAIOTurnover = 500 },
                new PassedPerfCheckCustomerEntity { CustID = 2, MemberWinloss = 200, MemberTotalTurnover = 2000, MemberAIOTurnover = 1000 }
            };
            var unpassedCustomers = new List<int> { 3 };

            dataAccess.GetSpecialSportBookVRAdjustmentCustomers(99, isTest).Returns(customerList);
            dataAccess.VerifyVRAdjustmentCustomers(Arg.Any<string>(), isTest)
                .Returns((new List<CustomerUpdFirstBetDateEntity>(), 
                         new List<CustomerUpdFirstBetDateEntity>(), 
                         passedCustomers, 
                         unpassedCustomers));

            // Act
            var result = service.AutoAdjustForBasketball(punterChunkSize, isTest);

            // Assert
            result.Should().Be(service);
            service.PunterBasketballUpdated.Should().Be(2);
            dataAccess.Received(1).UpdateVRAutoAdjustmentStatus(Arg.Is<string>(s => s.Contains("3")), 99, isTest);
        }

        [Theory]
        [InlineData(500, false)]
        public void AutoAdjustForSpecialSportsbook_WhenSuccess(int punterChunkSize, bool isTest)
        {
            // Arrange
            var customerList = new List<int> { 1, 2, 3 };
            var passedCustomers = new List<PassedPerfCheckCustomerEntity>
            {
                new PassedPerfCheckCustomerEntity { CustID = 1, MemberWinloss = 100, MemberTotalTurnover = 1000, MemberAIOTurnover = 500 },
                new PassedPerfCheckCustomerEntity { CustID = 2, MemberWinloss = 200, MemberTotalTurnover = 2000, MemberAIOTurnover = 1000 }
            };
            var unpassedCustomers = new List<int> { 3 };

            dataAccess.GetSpecialSportBookVRAdjustmentCustomers(100, isTest).Returns(customerList);
            dataAccess.VerifyVRAdjustmentCustomers(Arg.Any<string>(), isTest)
                .Returns((new List<CustomerUpdFirstBetDateEntity>(), 
                         new List<CustomerUpdFirstBetDateEntity>(), 
                         passedCustomers, 
                         unpassedCustomers));

            // Act
            var result = service.AutoAdjustForSpecialSportsbook(punterChunkSize, isTest);

            // Assert
            result.Should().Be(service);
            service.PunterSportsbookUpdated.Should().Be(2);
            dataAccess.Received(1).UpdateVRAutoAdjustmentStatus(Arg.Is<string>(s => s.Contains("3")), 100, isTest);
        }

        [Theory]
        [InlineData(500, false)]
        public void AutoAdjustForBasketball_WhenNoCustomers_ShouldNotUpdate(int punterChunkSize, bool isTest)
        {
            // Arrange
            dataAccess.GetSpecialSportBookVRAdjustmentCustomers(99, isTest).Returns(new List<int>());

            // Act
            var result = service.AutoAdjustForBasketball(punterChunkSize, isTest);

            // Assert
            result.Should().Be(service);
            service.PunterBasketballUpdated.Should().Be(0);
            dataAccess.DidNotReceive().UpdateVRAutoAdjustmentStatus(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<bool>());
        }

        [Theory]
        [InlineData(500, false)]
        public void AutoAdjustForSpecialSportsbook_WhenNoCustomers_ShouldNotUpdate(int punterChunkSize, bool isTest)
        {
            // Arrange
            dataAccess.GetSpecialSportBookVRAdjustmentCustomers(100, isTest).Returns(new List<int>());

            // Act
            var result = service.AutoAdjustForSpecialSportsbook(punterChunkSize, isTest);

            // Assert
            result.Should().Be(service);
            service.PunterSportsbookUpdated.Should().Be(0);
            dataAccess.DidNotReceive().UpdateVRAutoAdjustmentStatus(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<bool>());
        }
    }
}
