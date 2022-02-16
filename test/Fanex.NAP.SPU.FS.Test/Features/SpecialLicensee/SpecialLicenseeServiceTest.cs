namespace Fanex.NAP.SPU.VR.Tests.Features.SpecialLicensee
{
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class SpecialLicenseeServiceTest
    {
        private readonly ISpecialLicenseeDataAccess dataAccess;
        private readonly ISpecialLicenseeService service;

        public SpecialLicenseeServiceTest()
        {
            dataAccess = Substitute.For<ISpecialLicenseeDataAccess>();

            service = new SpecialLicenseeService(dataAccess);
        }

        [Theory]
        [InlineData(500, false)]
        public void AutoAdjustForBasketball_WhenSuccess(int punterChunkSize, bool isTest)
        {
            service.AutoAdjustForBasketball(punterChunkSize, isTest);
        }

        [Theory]
        [InlineData(500, false)]
        public void AutoAdjustForSpecialSportsbook_WhenSuccess(int punterChunkSize, bool isTest)
        {
            service.AutoAdjustForSpecialSportsbook(punterChunkSize, isTest);
        }
    }
}
