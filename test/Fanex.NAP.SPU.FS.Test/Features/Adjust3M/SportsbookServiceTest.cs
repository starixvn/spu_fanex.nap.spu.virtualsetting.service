namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using NSubstitute;
    using System;
    using Xunit;

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

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedSportbookInitModel), MemberType = typeof(Adjust3MExpectedData))]
        public void InitModel_WhenSuccess(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning)
        {
            // Act
            var result = service.InitModel(siteIds, superIds, adjustDate, virtualRateThresholdForWinning, string.Empty);

            // Assert
            Assert.IsAssignableFrom<ISportsbookService>(result);
        }

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedSportbookInitModel), MemberType = typeof(Adjust3MExpectedData))]
        public void AutoAdjustVirtualSettings_WhenSuccess(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning)
        {
            // Arrange            
            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = 1,
                IP = "123456",
                PunterChunkSize = 100
            };

            // Act
            service.InitModel(siteIds, superIds, adjustDate, virtualRateThresholdForWinning, string.Empty);

            var result = service.AutoAdjust(updateVirtualSettingEntity, 1);

            // Assert
            Assert.IsAssignableFrom<ISportsbookService>(result);
        }
    }
}
