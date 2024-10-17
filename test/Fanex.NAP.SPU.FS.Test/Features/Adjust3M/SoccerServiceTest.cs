namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class SoccerServiceTest
    {
        private readonly IAdjust3MDataAccess dataAccess;
        private readonly ISoccerService service;

        public SoccerServiceTest()
        {
            dataAccess = Substitute.For<IAdjust3MDataAccess>();

            service = new SoccerService(dataAccess);
        }

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedSoccerInitModel), MemberType = typeof(Adjust3MExpectedData))]
        public void InitModel_WhenSuccess(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning)
        {
            // Act
            var result = service.InitModel(siteIds, superIds, adjustDate, virtualRateThresholdForWinning, string.Empty);

            // Assert
            Assert.IsAssignableFrom<ISoccerService>(result);
        }

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedSoccerInitModel), MemberType = typeof(Adjust3MExpectedData))]
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

            var result = service.AutoAdjust(updateVirtualSettingEntity, 1, 1);

            // Assert
            Assert.IsAssignableFrom<ISoccerService>(result);
        }
    }
}
