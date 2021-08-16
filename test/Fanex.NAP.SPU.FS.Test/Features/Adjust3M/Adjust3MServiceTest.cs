namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M;   
    using FluentAssertions;
    using NSubstitute;
    using System;    
    using Xunit;

    public class Adjust3MServiceTest
    {
        private readonly VirtualSettingDataAccess dataAccess;       

        public Adjust3MServiceTest()
        {
            dataAccess = Substitute.For<VirtualSettingDataAccess>();
        }

        //[Theory]
        //[MemberData(nameof(Adjust3MExpectedData.ExpectedGetWinningPunterSoccerList), MemberType = typeof(Adjust3MExpectedData))]
        //public void GetWinningPunterSoccerList_WhenSuccess(double virtualRateThreshold)
        //{
        //    service.GetWinningPunterSoccerList(virtualRateThreshold);
        //}

        //[Theory]
        //[MemberData(nameof(Adjust3MExpectedData.ExpectedGetWinningPunterList), MemberType = typeof(Adjust3MExpectedData))]
        //public void GetWinningPunterList_WhenSuccess(double virtualRateThreshold)
        //{
        //    service.GetWinningPunterList(virtualRateThreshold);
        //}

        //[Theory]
        //[MemberData(nameof(Adjust3MExpectedData.ExpectedFilterPunterBaseOnCurrentVirtualRate), MemberType = typeof(Adjust3MExpectedData))]
        //public void FilterPunterBaseOnCurrentVirtualRate_WhenSuccess(short sportType, bool compareVROtherSports)
        //{
        //    service.FilterPunterBaseOnCurrentVirtualRate(sportType, compareVROtherSports);
        //}

        //[Theory]
        //[MemberData(nameof(Adjust3MExpectedData.ExpectedAdjustVirtualSettingForSuperProPunter), MemberType = typeof(Adjust3MExpectedData))]
        //public void AdjustVirtualSettingForSuperProPunter_WhenSuccess(int punterChunkSize, double virtualMultiplier)
        //{
        //    service.AdjustVirtualSettingForSuperProPunter(punterChunkSize, virtualMultiplier);
        //}

        //[Theory]
        //[MemberData(nameof(Adjust3MExpectedData.ExpectedAdjustVirtualSettingForNormal), MemberType = typeof(Adjust3MExpectedData))]
        //public void AdjustVirtualSettingForNormal_WhenSuccess(int punterChunkSize, double virtualMultiplier)
        //{
        //    service.AdjustVirtualSettingForNormal(punterChunkSize, virtualMultiplier);
        //}

    }
}
