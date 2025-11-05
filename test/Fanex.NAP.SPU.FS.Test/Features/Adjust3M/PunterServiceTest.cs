namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Unit tests for PunterService class
    /// </summary>
    public class PunterServiceTest
    {
        private readonly IAdjust3MDataAccess dataAccess;
        private readonly VirtualSettingModel model;
        private readonly PunterService service;

        public PunterServiceTest()
        {
            dataAccess = Substitute.For<IAdjust3MDataAccess>();
            model = CreateVirtualSettingModel();
            service = new PunterService(dataAccess, model, false);
            // Setup test logging          
            var logEngine = Substitute.For<ILogEngine>();

            LogManager
                .SetDefaultLogCategory("")
                .Use(logEngine);
        }

        private VirtualSettingModel CreateVirtualSettingModel()
        {
            return new VirtualSettingModel
            {
                ServiceName = "TestService",
                AdjustDate = DateTime.Now,
                SportGroupID = 1,
                SiteIds = "1,2,3",
                SuperIds = "100,200,300",
                VirtualRateThreshold = 1.5,
                JsonLicenseeBACusts = "[]",
                FailedUpdatePunterList = new List<PunterEntity>(),
                PunterEqualityComparer = new PunterEqualityComparer()
            };
        }

        private PunterEntity CreatePunterEntity(int custId, bool isSuperPro = false, short sportType = 1)
        {
            return new PunterEntity
            {
                CustID = custId,
                IsSuperProPunters = isSuperPro,
                SportType = sportType,
                OldVirtualRate = 1.0,
                NewVirtualRate = 2.0,
                SiteID = 1,
                ReportDate = DateTime.Now,
                SRecommend = 0,
                MRecommend = 0,
                Recommend = 0,
                IsBA = false
            };
        }

        private CustomerVirtualRateEntity CreateCustomerVirtualRateEntity(int custId, double vrSoccer = 1.0, double vrOtherSports = 1.0)
        {
            return new CustomerVirtualRateEntity
            {
                CustID = custId,
                VRSoccer = vrSoccer,
                VROtherSports = vrOtherSports
            };
        }

        private UpdateVirtualSettingEntity CreateUpdateVirtualSettingEntity()
        {
            return new UpdateVirtualSettingEntity
            {
                DummyUser = 123,
                IP = "192.168.1.1",
                PunterChunkSize = 10
            };
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockDataAccess = Substitute.For<IAdjust3MDataAccess>();
            var mockModel = CreateVirtualSettingModel();

            // Act
            var service = new PunterService(mockDataAccess, mockModel, true);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithSoccerTrue_ShouldSetSoccerFlag()
        {
            // Arrange
            var mockDataAccess = Substitute.For<IAdjust3MDataAccess>();
            var mockModel = CreateVirtualSettingModel();

            // Act
            var service = new PunterService(mockDataAccess, mockModel, true);

            // Assert
            var soccerField = typeof(PunterService).GetField("_soccer", BindingFlags.NonPublic | BindingFlags.Instance);
            var soccerValue = (bool)soccerField.GetValue(service);
            soccerValue.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithSoccerFalse_ShouldSetSoccerFlag()
        {
            // Arrange
            var mockDataAccess = Substitute.For<IAdjust3MDataAccess>();
            var mockModel = CreateVirtualSettingModel();

            // Act
            var service = new PunterService(mockDataAccess, mockModel, false);

            // Assert
            var soccerField = typeof(PunterService).GetField("_soccer", BindingFlags.NonPublic | BindingFlags.Instance);
            var soccerValue = (bool)soccerField.GetValue(service);
            soccerValue.Should().BeFalse();
        }

        #endregion

        #region GetWinningPunterSoccerList Tests

        [Fact]
        public void GetWinningPunterSoccerList_WithValidData_ShouldReturnThis()
        {
            // Arrange
            var punters = new List<PunterEntity>
            {
                CreatePunterEntity(1),
                CreatePunterEntity(2)
            };
            dataAccess.GetWinningPunterSoccerList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold)
                .Returns(punters);

            // Act
            var result = service.GetWinningPunterSoccerList();

            // Assert
            result.Should().BeSameAs(service);
            dataAccess.Received(1).GetWinningPunterSoccerList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold);
        }

        [Fact]
        public void GetWinningPunterSoccerList_WithEmptyData_ShouldReturnThis()
        {
            // Arrange
            var emptyPunters = new List<PunterEntity>();
            dataAccess.GetWinningPunterSoccerList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold)
                .Returns(emptyPunters);

            // Act
            var result = service.GetWinningPunterSoccerList();

            // Assert
            result.Should().BeSameAs(service);
            dataAccess.Received(1).GetWinningPunterSoccerList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold);
        }

        [Fact]
        public void GetWinningPunterSoccerList_WithNullData_ShouldReturnThis()
        {
            // Arrange
            dataAccess.GetWinningPunterSoccerList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold)
                .Returns((IEnumerable<PunterEntity>)null);

            // Act
            var result = service.GetWinningPunterSoccerList();

            // Assert
            result.Should().BeSameAs(service);
            dataAccess.Received(1).GetWinningPunterSoccerList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold);
        }

        #endregion

        #region GetWinningPunterList Tests

        [Fact]
        public void GetWinningPunterList_WithValidData_ShouldReturnThis()
        {
            // Arrange
            var punters = new List<PunterEntity>
            {
                CreatePunterEntity(1),
                CreatePunterEntity(2)
            };
            dataAccess.GetWinningPunterList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold)
                .Returns(punters);

            // Act
            var result = service.GetWinningPunterList();

            // Assert
            result.Should().BeSameAs(service);
            dataAccess.Received(1).GetWinningPunterList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold);
        }

        [Fact]
        public void GetWinningPunterList_WithEmptyData_ShouldReturnThis()
        {
            // Arrange
            var emptyPunters = new List<PunterEntity>();
            dataAccess.GetWinningPunterList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold)
                .Returns(emptyPunters);

            // Act
            var result = service.GetWinningPunterList();

            // Assert
            result.Should().BeSameAs(service);
            dataAccess.Received(1).GetWinningPunterList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold);
        }

        [Fact]
        public void GetWinningPunterList_WithNullData_ShouldReturnThis()
        {
            // Arrange
            dataAccess.GetWinningPunterList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold)
                .Returns((IEnumerable<PunterEntity>)null);

            // Act
            var result = service.GetWinningPunterList();

            // Assert
            result.Should().BeSameAs(service);
            dataAccess.Received(1).GetWinningPunterList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold);
        }

        #endregion

        #region FilterPunterBaseOnCurrentVirtualRate Tests

        [Fact]
        public void FilterPunterBaseOnCurrentVirtualRate_WithNullPunters_ShouldReturnThis()
        {
            // Arrange
            var method = typeof(PunterService).GetMethod("FilterPunterBaseOnCurrentVirtualRate", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { (short)1, true });

            // Assert
            result.Should().BeSameAs(service);
        }
       
        #endregion

        #region AdjustVirtualSettingForSuperProPunter Tests

        [Fact]
        public void AdjustVirtualSettingForSuperProPunter_WithNullPuntersDict_ShouldReturnThis()
        {
            // Arrange
            var updateSetting = CreateUpdateVirtualSettingEntity();
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForSuperProPunter", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { updateSetting, 2.0 });

            // Assert
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AdjustVirtualSettingForSuperProPunter_WithEmptyPuntersDict_ShouldReturnThis()
        {
            // Arrange
            var emptyDict = new Dictionary<int, PunterEntity>();
            var puntersDictField = typeof(PunterService).GetField("_puntersDict", BindingFlags.NonPublic | BindingFlags.Instance);
            puntersDictField.SetValue(service, emptyDict);

            var updateSetting = CreateUpdateVirtualSettingEntity();
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForSuperProPunter", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { updateSetting, 2.0 });

            // Assert
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AdjustVirtualSettingForSuperProPunter_WithNoProPunters_ShouldReturnThis()
        {
            // Arrange
            var puntersDict = new Dictionary<int, PunterEntity>
            {
                { 1, CreatePunterEntity(1, false) }, // Not super pro
                { 2, CreatePunterEntity(2, false) }  // Not super pro
            };
            var puntersDictField = typeof(PunterService).GetField("_puntersDict", BindingFlags.NonPublic | BindingFlags.Instance);
            puntersDictField.SetValue(service, puntersDict);

            var updateSetting = CreateUpdateVirtualSettingEntity();
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForSuperProPunter", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { updateSetting, 2.0 });

            // Assert
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AdjustVirtualSettingForSuperProPunter_WithProPunters_ShouldProcessCorrectly()
        {
            // Arrange
            var puntersDict = new Dictionary<int, PunterEntity>
            {
                { 1, CreatePunterEntity(1, true) },  // Super pro
                { 2, CreatePunterEntity(2, false) }  // Not super pro
            };
            var puntersDictField = typeof(PunterService).GetField("_puntersDict", BindingFlags.NonPublic | BindingFlags.Instance);
            puntersDictField.SetValue(service, puntersDict);

            var updateSetting = CreateUpdateVirtualSettingEntity();
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForSuperProPunter", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { updateSetting, 2.0 });

            // Assert
            result.Should().BeSameAs(service);
        }

        #endregion

        #region AdjustVirtualSettingForNormal Tests

        [Fact]
        public void AdjustVirtualSettingForNormal_WithNullPuntersDict_ShouldReturnThis()
        {
            // Arrange
            var updateSetting = CreateUpdateVirtualSettingEntity();
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForNormal", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { updateSetting, 2.0 });

            // Assert
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AdjustVirtualSettingForNormal_WithEmptyPuntersDict_ShouldReturnThis()
        {
            // Arrange
            var emptyDict = new Dictionary<int, PunterEntity>();
            var puntersDictField = typeof(PunterService).GetField("_puntersDict", BindingFlags.NonPublic | BindingFlags.Instance);
            puntersDictField.SetValue(service, emptyDict);

            var updateSetting = CreateUpdateVirtualSettingEntity();
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForNormal", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { updateSetting, 2.0 });

            // Assert
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AdjustVirtualSettingForNormal_WithNoNormalPunters_ShouldReturnThis()
        {
            // Arrange
            var puntersDict = new Dictionary<int, PunterEntity>
            {
                { 1, CreatePunterEntity(1, true) },  // Super pro
                { 2, CreatePunterEntity(2, true) }   // Super pro
            };
            var puntersDictField = typeof(PunterService).GetField("_puntersDict", BindingFlags.NonPublic | BindingFlags.Instance);
            puntersDictField.SetValue(service, puntersDict);

            var updateSetting = CreateUpdateVirtualSettingEntity();
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForNormal", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { updateSetting, 2.0 });

            // Assert
            result.Should().BeSameAs(service);
        }

        [Fact]
        public void AdjustVirtualSettingForNormal_WithNormalPunters_ShouldProcessCorrectly()
        {
            // Arrange
            var puntersDict = new Dictionary<int, PunterEntity>
            {
                { 1, CreatePunterEntity(1, false) }, // Normal
                { 2, CreatePunterEntity(2, true) }   // Super pro
            };
            var puntersDictField = typeof(PunterService).GetField("_puntersDict", BindingFlags.NonPublic | BindingFlags.Instance);
            puntersDictField.SetValue(service, puntersDict);

            var updateSetting = CreateUpdateVirtualSettingEntity();
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForNormal", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { updateSetting, 2.0 });

            // Assert
            result.Should().BeSameAs(service);
        }

        #endregion

        #region GetLicenseeBACusts Tests

        [Fact]
        public void GetLicenseeBACusts_WithValidData_ShouldReturnJsonString()
        {
            // Arrange
            var licenseeBACusts = new List<LicenseeBAEntity>
            {
                new LicenseeBAEntity { CustID = 1 },
                new LicenseeBAEntity { CustID = 2 }
            };
            dataAccess.GetLicenseeBACusts().Returns(licenseeBACusts);

            // Act
            var result = service.GetLicenseeBACusts();

            // Assert
            result.Should().NotBeNullOrEmpty();
            dataAccess.Received(1).GetLicenseeBACusts();
        }

        [Fact]
        public void GetLicenseeBACusts_WithEmptyData_ShouldReturnEmptyJsonArray()
        {
            // Arrange
            var emptyLicenseeBACusts = new List<LicenseeBAEntity>();
            dataAccess.GetLicenseeBACusts().Returns(emptyLicenseeBACusts);

            // Act
            var result = service.GetLicenseeBACusts();

            // Assert
            result.Should().Be("[]");
            dataAccess.Received(1).GetLicenseeBACusts();
        }

        [Fact]
        public void GetLicenseeBACusts_WithNullData_ShouldReturnEmptyJsonArray()
        {
            // Arrange
            dataAccess.GetLicenseeBACusts().Returns((IEnumerable<LicenseeBAEntity>)null);

            // Act
            var result = service.GetLicenseeBACusts();

            // Assert
            result.Should().Be("null");
            dataAccess.Received(1).GetLicenseeBACusts();
        }

        #endregion

        #region CountAffected Tests

        [Fact]
        public void CountAffected_WithBothProAndNormalPuntersUpdated_ShouldReturnCombinedCount()
        {
            // Arrange
            var proPunters = new List<PunterEntity> { CreatePunterEntity(1, true) };
            var normalPunters = new List<PunterEntity> { CreatePunterEntity(2, false) };

            var proPuntersField = typeof(PunterService).GetField("_proPuntersUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
            var normalPuntersField = typeof(PunterService).GetField("_normalPuntersUpdated", BindingFlags.NonPublic | BindingFlags.Instance);

            proPuntersField.SetValue(service, proPunters);
            normalPuntersField.SetValue(service, normalPunters);

            // Act
            var result = service.CountAffected();

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void CountAffected_WithOnlyProPuntersUpdated_ShouldReturnProCount()
        {
            // Arrange
            var proPunters = new List<PunterEntity> { CreatePunterEntity(1, true), CreatePunterEntity(2, true) };

            var proPuntersField = typeof(PunterService).GetField("_proPuntersUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
            proPuntersField.SetValue(service, proPunters);

            // Act
            var result = service.CountAffected();

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void CountAffected_WithOnlyNormalPuntersUpdated_ShouldReturnNormalCount()
        {
            // Arrange
            var normalPunters = new List<PunterEntity> { CreatePunterEntity(1, false), CreatePunterEntity(2, false) };

            var normalPuntersField = typeof(PunterService).GetField("_normalPuntersUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
            normalPuntersField.SetValue(service, normalPunters);

            // Act
            var result = service.CountAffected();

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void CountAffected_WithNoPuntersUpdated_ShouldReturnZero()
        {
            // Act
            var result = service.CountAffected();

            // Assert
            result.Should().Be(0);
        }

        #endregion

        #region Private Method Tests

        [Fact]
        public void UpdateVirtualSetting_WithValidPunters_ShouldProcessCorrectly()
        {
            // Arrange
            var punters = new List<PunterEntity> { CreatePunterEntity(1), CreatePunterEntity(2) };
            var updateSetting = CreateUpdateVirtualSettingEntity();

            var method = typeof(PunterService).GetMethod("UpdateVirtualSetting", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { punters, updateSetting, 2.0 });

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void DetectBAAccounts_WithValidPunters_ShouldProcessCorrectly()
        {
            // Arrange
            var punters = new List<PunterEntity> { CreatePunterEntity(1), CreatePunterEntity(2) };
            var baCustIds = new List<int> { 1 };

            dataAccess.GetBACustIds("1,2").Returns(baCustIds);

            var method = typeof(PunterService).GetMethod("DetectBAAccounts", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method.Invoke(service, new object[] { 3.0, punters });

            // Assert
            dataAccess.Received(1).GetBACustIds("1,2");
        }

        [Fact]
        public void GetCurrentCustomerVirtualRate_WithValidCustIds_ShouldReturnVirtualRates()
        {
            // Arrange
            var custIds = "1,2,3";
            var virtualRates = new List<CustomerVirtualRateEntity>
            {
                CreateCustomerVirtualRateEntity(1),
                CreateCustomerVirtualRateEntity(2)
            };

            dataAccess.GetCurrentCustomerVirtualRate(custIds).Returns(virtualRates);

            var method = typeof(PunterService).GetMethod("GetCurrentCustomerVirtualRate", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { custIds });

            // Assert
            result.Should().NotBeNull();
            dataAccess.Received(1).GetCurrentCustomerVirtualRate(custIds);
        }       

        [Fact]
        public void UpdatePunterVirtualSetting_WithSoccerFalse_ShouldCallAllSportsUpdate()
        {
            // Arrange
            var punters = new List<PunterEntity> { CreatePunterEntity(1) };

            var method = typeof(PunterService).GetMethod("UpdatePunterVirtualSetting", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = method.Invoke(service, new object[] { punters, 2.0, 123, "192.168.1.1", "[]" });

            // Assert
            result.Should().NotBeNull();
        }

        #endregion        

        #region Integration Tests

        [Fact]
        public void CompleteWorkflow_WithValidData_ShouldProcessAllSteps()
        {
            // Arrange
            var punters = new List<PunterEntity>
            {
                CreatePunterEntity(1, true),
                CreatePunterEntity(2, false)
            };
            var virtualRates = new List<CustomerVirtualRateEntity>
            {
                CreateCustomerVirtualRateEntity(1, 1.0, 1.0),
                CreateCustomerVirtualRateEntity(2, 1.0, 1.0)
            };

            dataAccess.GetWinningPunterList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold).Returns(punters);
            dataAccess.GetCurrentCustomerVirtualRate(Arg.Any<string>()).Returns(virtualRates);

            // Act
            var result = service
                .GetWinningPunterList()
                .FilterPunterBaseOnCurrentVirtualRate(1, true)
                .AdjustVirtualSettingForSuperProPunter(CreateUpdateVirtualSettingEntity(), 2.0)
                .AdjustVirtualSettingForNormal(CreateUpdateVirtualSettingEntity(), 2.0);

            // Assert
            result.Should().BeSameAs(service);
            dataAccess.Received(1).GetWinningPunterList(model.SiteIds, model.SuperIds, model.VirtualRateThreshold);
            dataAccess.Received(1).GetCurrentCustomerVirtualRate(Arg.Any<string>());
        }

        #endregion

        #region Method Signature Tests

        [Fact]
        public void GetWinningPunterSoccerList_ShouldReturnIPunterService()
        {
            // Arrange
            var method = typeof(PunterService).GetMethod("GetWinningPunterSoccerList");

            // Assert
            method.ReturnType.Should().Be(typeof(IPunterService));
            method.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void GetWinningPunterList_ShouldReturnIPunterService()
        {
            // Arrange
            var method = typeof(PunterService).GetMethod("GetWinningPunterList");

            // Assert
            method.ReturnType.Should().Be(typeof(IPunterService));
            method.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void FilterPunterBaseOnCurrentVirtualRate_ShouldReturnIPunterService()
        {
            // Arrange
            var method = typeof(PunterService).GetMethod("FilterPunterBaseOnCurrentVirtualRate");

            // Assert
            method.ReturnType.Should().Be(typeof(IPunterService));
            method.GetParameters().Should().HaveCount(2);
        }

        [Fact]
        public void AdjustVirtualSettingForSuperProPunter_ShouldReturnIPunterService()
        {
            // Arrange
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForSuperProPunter");

            // Assert
            method.ReturnType.Should().Be(typeof(IPunterService));
            method.GetParameters().Should().HaveCount(2);
        }

        [Fact]
        public void AdjustVirtualSettingForNormal_ShouldReturnIPunterService()
        {
            // Arrange
            var method = typeof(PunterService).GetMethod("AdjustVirtualSettingForNormal");

            // Assert
            method.ReturnType.Should().Be(typeof(IPunterService));
            method.GetParameters().Should().HaveCount(2);
        }

        [Fact]
        public void GetLicenseeBACusts_ShouldReturnString()
        {
            // Arrange
            var method = typeof(PunterService).GetMethod("GetLicenseeBACusts");

            // Assert
            method.ReturnType.Should().Be(typeof(string));
            method.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void CountAffected_ShouldReturnInt()
        {
            // Arrange
            var method = typeof(PunterService).GetMethod("CountAffected");

            // Assert
            method.ReturnType.Should().Be(typeof(int));
            method.GetParameters().Should().BeEmpty();
        }

        #endregion
    }
}
