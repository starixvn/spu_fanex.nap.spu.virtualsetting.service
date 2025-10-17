namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Unit tests for SoccerService class
    /// </summary>
    public class SoccerServiceTest
    {
        private readonly IAdjust3MDataAccess dataAccess;
        private readonly ISoccerService service;

        public SoccerServiceTest()
        {
            // Setup test logging          
            var logEngine = Substitute.For<ILogEngine>();

            LogManager
                .SetDefaultLogCategory("")
                .Use(logEngine);

            dataAccess = Substitute.For<IAdjust3MDataAccess>();
            service = new SoccerService(dataAccess);
        }

        #region InitModel Tests       
        [Fact]
        public void InitModel_WhenCalledWithValidParameters_ShouldInitializePunterService()
        {
            // Arrange
            var siteIds = "1,2,3";
            var superIds = "100,200";
            var adjustDate = DateTime.Now;
            var virtualRateThreshold = 1.5;
            var jsonLicenseeBACusts = "{\"test\":\"data\"}";

            // Act
            var result = service.InitModel(siteIds, superIds, adjustDate, virtualRateThreshold, jsonLicenseeBACusts);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void InitModel_WhenCalledWithEmptyParameters_ShouldStillReturnService()
        {
            // Arrange
            var siteIds = "";
            var superIds = "";
            var adjustDate = DateTime.MinValue;
            var virtualRateThreshold = 0.0;
            var jsonLicenseeBACusts = "";

            // Act
            var result = service.InitModel(siteIds, superIds, adjustDate, virtualRateThreshold, jsonLicenseeBACusts);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void InitModel_WhenCalledWithNullParameters_ShouldStillReturnService()
        {
            // Arrange
            string siteIds = null;
            string superIds = null;
            var adjustDate = DateTime.Now;
            var virtualRateThreshold = 1.0;
            string jsonLicenseeBACusts = null;

            // Act
            var result = service.InitModel(siteIds, superIds, adjustDate, virtualRateThreshold, jsonLicenseeBACusts);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        #endregion

        #region AutoAdjust Tests      

        [Fact]
        public void AutoAdjust_WhenPunterServiceIsNull_ShouldReturnServiceWithoutError()
        {
            // Arrange
            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = 1,
                IP = "192.168.1.1",
                PunterChunkSize = 100
            };

            // Act
            var result = service.AutoAdjust(updateVirtualSettingEntity, 1.5, 2.0);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AutoAdjust_WhenCalledWithZeroMultipliers_ShouldReturnService()
        {
            // Arrange
            service.InitModel("1,2", "100,200", DateTime.Now, 1.0, "");
            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = 1,
                IP = "192.168.1.1",
                PunterChunkSize = 100
            };

            // Act
            var result = service.AutoAdjust(updateVirtualSettingEntity, 0.0, 0.0);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AutoAdjust_WhenCalledWithNegativeMultipliers_ShouldReturnService()
        {
            // Arrange
            service.InitModel("1,2", "100,200", DateTime.Now, 1.0, "");
            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = 1,
                IP = "192.168.1.1",
                PunterChunkSize = 100
            };

            // Act
            var result = service.AutoAdjust(updateVirtualSettingEntity, -1.0, -2.0);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AutoAdjust_WhenCalledWithNullUpdateEntity_ShouldReturnServiceWithoutError()
        {
            // Arrange
            service.InitModel("1,2", "100,200", DateTime.Now, 1.0, "");

            // Act
            var result = service.AutoAdjust(null, 1.0, 2.0);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        #endregion

        #region GetLicenseeBACusts Tests

        [Fact]
        public void GetLicenseeBACusts_WhenDataAccessReturnsData_ShouldReturnJsonString()
        {
            // Arrange
            var expectedLicenseeBACusts = new List<LicenseeBAEntity>
            {
                new LicenseeBAEntity { CustID = 123 },
                new LicenseeBAEntity { CustID = 456 }
            };
            dataAccess.GetLicenseeBACusts().Returns(expectedLicenseeBACusts);

            // Act
            var result = service.GetLicenseeBACusts();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("123");
            result.Should().Contain("456");
            dataAccess.Received(1).GetLicenseeBACusts();
        }

        [Fact]
        public void GetLicenseeBACusts_WhenDataAccessReturnsEmptyList_ShouldReturnEmptyJsonArray()
        {
            // Arrange
            var emptyList = new List<LicenseeBAEntity>();
            dataAccess.GetLicenseeBACusts().Returns(emptyList);

            // Act
            var result = service.GetLicenseeBACusts();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("[]");
            dataAccess.Received(1).GetLicenseeBACusts();
        }

        [Fact]
        public void GetLicenseeBACusts_WhenDataAccessReturnsNull_ShouldReturnNullJson()
        {
            // Arrange
            dataAccess.GetLicenseeBACusts().Returns((IEnumerable<LicenseeBAEntity>)null);

            // Act
            var result = service.GetLicenseeBACusts();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("null");
            dataAccess.Received(1).GetLicenseeBACusts();
        }

        #endregion

        #region GetCountAffected Tests

        [Fact]
        public void GetCountAffected_WhenPunterServiceIsNotInitialized_ShouldThrowNullReferenceException()
        {
            // Act & Assert
            Assert.Throws<NullReferenceException>(() => service.GetCountAffected());
        }

        [Fact]
        public void GetCountAffected_WhenPunterServiceIsInitialized_ShouldReturnCountFromPunterService()
        {
            // Arrange
            service.InitModel("1,2", "100,200", DateTime.Now, 1.0, "");

            // Act
            var result = service.GetCountAffected();

            // Assert
            result.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public void GetCountAffected_WhenCalledMultipleTimes_ShouldReturnConsistentCount()
        {
            // Arrange
            service.InitModel("1,2", "100,200", DateTime.Now, 1.0, "");

            // Act
            var result1 = service.GetCountAffected();
            var result2 = service.GetCountAffected();

            // Assert
            result1.Should().Be(result2);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void FullWorkflow_WhenCalledInSequence_ShouldWorkCorrectly()
        {
            // Arrange
            var siteIds = "1,2,3";
            var superIds = "100,200,300";
            var adjustDate = DateTime.Now;
            var virtualRateThreshold = 1.5;
            var jsonLicenseeBACusts = "{\"test\":\"data\"}";

            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = 1,
                IP = "192.168.1.1",
                PunterChunkSize = 100
            };

            var expectedLicenseeBACusts = new List<LicenseeBAEntity>
            {
                new LicenseeBAEntity { CustID = 123 }
            };
            dataAccess.GetLicenseeBACusts().Returns(expectedLicenseeBACusts);

            // Act
            var initResult = service.InitModel(siteIds, superIds, adjustDate, virtualRateThreshold, jsonLicenseeBACusts);
            var autoAdjustResult = service.AutoAdjust(updateVirtualSettingEntity, 1.5, 2.0);
            var licenseeResult = service.GetLicenseeBACusts();
            var countResult = service.GetCountAffected();

            // Assert
            initResult.Should().BeSameAs(service);
            autoAdjustResult.Should().BeSameAs(service);
            licenseeResult.Should().NotBeNullOrEmpty();
            countResult.Should().BeGreaterOrEqualTo(0);
        }

        #endregion
    }
}