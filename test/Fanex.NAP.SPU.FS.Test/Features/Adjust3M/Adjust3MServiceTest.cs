namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M;   
    using FluentAssertions;
    using NSubstitute;
    using System;    
    using Xunit;

    public class Adjust3MServiceTest
    {
        private readonly IAdjust3MDataAccess dataAccess;
        private readonly IAdjust3MService service;

        public Adjust3MServiceTest()
        {
            dataAccess = Substitute.For<IAdjust3MDataAccess>();

            service = new Adjust3MService(dataAccess);
        }

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedGetWinningPunterSoccerList), MemberType = typeof(Adjust3MExpectedData))]
        public void GetWinningPunterSoccerList_WhenSuccess(string siteIds, string superIds, double virtualRateThreshold)
        {
            service.GetWinningPunterSoccerList(siteIds, superIds, virtualRateThreshold);
        }

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedGetWinningPunterList), MemberType = typeof(Adjust3MExpectedData))]
        public void GetWinningPunterList_WhenSuccess(string siteIds, string superIds, double virtualRateThreshold)
        {
            service.GetWinningPunterList(siteIds, superIds, virtualRateThreshold);
        }

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedFilterPunterBaseOnCurrentVirtualRate), MemberType = typeof(Adjust3MExpectedData))]
        public void FilterPunterBaseOnCurrentVirtualRate_WhenSuccess(short sportType, bool compareVROtherSports)
        {
            service.FilterPunterBaseOnCurrentVirtualRate(sportType, compareVROtherSports);
        }

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedAdjustVirtualSettingForSuperProPunter), MemberType = typeof(Adjust3MExpectedData))]
        public void AdjustVirtualSettingForSuperProPunter_WhenSuccess(int dummyUser, string ip, int punterChunkSize, double virtualMultiplier)
        {
            service.AdjustVirtualSettingForSuperProPunter(dummyUser, ip, punterChunkSize, virtualMultiplier);
        }

        [Theory]
        [MemberData(nameof(Adjust3MExpectedData.ExpectedAdjustVirtualSettingForNormal), MemberType = typeof(Adjust3MExpectedData))]
        public void AdjustVirtualSettingForNormal_WhenSuccess(int dummyUser, string ip, int punterChunkSize, double virtualMultiplier)
        {
            service.AdjustVirtualSettingForNormal(dummyUser, ip, punterChunkSize, virtualMultiplier);
        }

    }
}
