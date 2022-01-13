namespace Fanex.NAP.SPU.VR.Tests.Features.MemberBigStake
{
    using Fanex.NAP.SPU.VR.Features.MemberBigStake;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class MemberBigStakeServiceTest
    {
        private readonly IMemberBigStakeDataAccess dataAccess;
        private readonly IMemberBigStakeService service;

        public MemberBigStakeServiceTest()
        {
            dataAccess = Substitute.For<IMemberBigStakeDataAccess>();

            service = new MemberBigStakeService(dataAccess);
        }

        [Fact]
        public void InsertMemberBigStake_WhenSuccess()
        {
            service.InsertMemberBigStake(false);
        }

        [Fact]
        public void GetMemberBigStakePerformance_WhenSuccess()
        {
            var expectedResult = MemberBigStakeExpectedData.ExpectedMemberBigStakeModel;

            dataAccess.GetMemberBigStakePerformance(false).Returns(expectedResult);

            var result = service.GetMemberBigStakePerformance(false);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [MemberData(nameof(MemberBigStakeExpectedData.ExpectedUpdateMemberBigStakeFirstBetDate), MemberType = typeof(MemberBigStakeExpectedData))]
        public void UpdateMemberBigStakeFirstBetDate_WhenSuccess(IEnumerable<long> customerList, int type, DateTime betDate, bool isTest)
        {
            service.UpdateMemberBigStakeFirstBetDate(customerList, type, betDate, isTest);
        }

        [Theory]
        [MemberData(nameof(MemberBigStakeExpectedData.ExpectedUpdateWhitelistMemberBigStake), MemberType = typeof(MemberBigStakeExpectedData))]
        public void UpdateWhitelistMemberBigStake_WhenSuccess(IEnumerable<MemberBigStakePerformanceStatusModel> customerList, int type)
        {
            service.UpdateWhitelistMemberBigStake(customerList, type, false);
        }

        [Fact]
        public void GetBigCreditMemberForAutomail_WhenSuccess()
        {
            var expectedResult = MemberBigStakeExpectedData.ExpectedGetBigCreditMemberForAutomail;

            dataAccess.GetBigCreditMemberForAutomail().Returns(expectedResult);

            var result = service.GetBigCreditMemberForAutomail();

            result.Should().BeEquivalentTo(expectedResult);
        }

    }
}
