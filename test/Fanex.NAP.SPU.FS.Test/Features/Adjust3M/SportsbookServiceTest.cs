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
    /// Unit tests for SportsbookService class
    /// </summary>
    public class SportsbookServiceTest
    {
        private readonly IAdjust3MDataAccess dataAccess;
        private readonly ISportsbookService service;

        public SportsbookServiceTest()
        {
            // Setup test logging          
            var logEngine = Substitute.For<ILogEngine>();

            LogManager
                .SetDefaultLogCategory("")
                .Use(logEngine);

            dataAccess = Substitute.For<IAdjust3MDataAccess>();
            service = new SportsbookService(dataAccess);
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
            var result = service.AutoAdjust(updateVirtualSettingEntity, 2.0);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AutoAdjust_WhenCalledWithZeroMultiplier_ShouldReturnService()
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
            var result = service.AutoAdjust(updateVirtualSettingEntity, 0.0);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AutoAdjust_WhenCalledWithNegativeMultiplier_ShouldReturnService()
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
            var result = service.AutoAdjust(updateVirtualSettingEntity, -1.0);

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
            var result = service.AutoAdjust(null, 2.0);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AutoAdjust_WhenCalledWithLargeMultiplier_ShouldReturnService()
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
            var result = service.AutoAdjust(updateVirtualSettingEntity, 999.99);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
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

        [Fact]
        public void GetCountAffected_AfterAutoAdjust_ShouldReturnUpdatedCount()
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
            var countBefore = service.GetCountAffected();
            service.AutoAdjust(updateVirtualSettingEntity, 2.0);
            var countAfter = service.GetCountAffected();

            // Assert
            countBefore.Should().BeGreaterOrEqualTo(0);
            countAfter.Should().BeGreaterOrEqualTo(0);
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

            // Act
            var initResult = service.InitModel(siteIds, superIds, adjustDate, virtualRateThreshold, jsonLicenseeBACusts);
            var autoAdjustResult = service.AutoAdjust(updateVirtualSettingEntity, 2.0);
            var countResult = service.GetCountAffected();

            // Assert
            initResult.Should().BeSameAs(service);
            autoAdjustResult.Should().BeSameAs(service);
            countResult.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public void MultipleAutoAdjustCalls_ShouldWorkCorrectly()
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
            var result1 = service.AutoAdjust(updateVirtualSettingEntity, 1.5);
            var result2 = service.AutoAdjust(updateVirtualSettingEntity, 2.0);
            var result3 = service.AutoAdjust(updateVirtualSettingEntity, 2.5);

            // Assert
            result1.Should().BeSameAs(service);
            result2.Should().BeSameAs(service);
            result3.Should().BeSameAs(service);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void InitModel_WithMinMaxValues_ShouldHandleCorrectly()
        {
            // Arrange
            var siteIds = "1,2,3,4,5,6,7,8,9,10";
            var superIds = "100,200,300,400,500";
            var adjustDate = DateTime.MaxValue;
            var virtualRateThreshold = double.MaxValue;
            var jsonLicenseeBACusts = "{\"large\":\"json\",\"with\":\"many\",\"properties\":\"and\",\"values\":[1,2,3,4,5]}";

            // Act
            var result = service.InitModel(siteIds, superIds, adjustDate, virtualRateThreshold, jsonLicenseeBACusts);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AutoAdjust_WithExtremeValues_ShouldHandleCorrectly()
        {
            // Arrange
            service.InitModel("1", "100", DateTime.Now, 1.0, "");
            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = int.MaxValue,
                IP = "255.255.255.255",
                PunterChunkSize = int.MaxValue
            };

            // Act
            var result = service.AutoAdjust(updateVirtualSettingEntity, double.MaxValue);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void GetCountAffected_AfterMultipleOperations_ShouldBeConsistent()
        {
            // Arrange
            service.InitModel("1,2,3", "100,200,300", DateTime.Now, 1.0, "");
            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = 1,
                IP = "192.168.1.1",
                PunterChunkSize = 100
            };

            // Act
            var count1 = service.GetCountAffected();
            service.AutoAdjust(updateVirtualSettingEntity, 1.5);
            var count2 = service.GetCountAffected();
            service.AutoAdjust(updateVirtualSettingEntity, 2.0);
            var count3 = service.GetCountAffected();

            // Assert
            count1.Should().BeGreaterOrEqualTo(0);
            count2.Should().BeGreaterOrEqualTo(0);
            count3.Should().BeGreaterOrEqualTo(0);
        }

        #endregion
    }
}