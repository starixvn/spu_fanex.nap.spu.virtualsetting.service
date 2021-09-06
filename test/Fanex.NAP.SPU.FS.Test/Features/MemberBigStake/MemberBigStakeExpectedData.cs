namespace Fanex.NAP.SPU.VR.Tests.Features.MemberBigStake
{
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using System;
    using System.Collections.Generic;

    public static class MemberBigStakeExpectedData
    {
        public static MemberBigStakePerformanceModel ExpectedMemberBigStakeModel
        {
            get
            {
                return new MemberBigStakePerformanceModel
                {
                    FirstBetDateList = new List<MemberBigStakePerformanceFirstDayEntity>(),
                    StatusList = new List<MemberBigStakePerformanceStatusEntity>()
                };
            }
        }
        
        public static IEnumerable<object[]> ExpectedUpdateMemberBigStakeFirstBetDate
        {
            get
            {
                return new List<object[]> {
                    new object[] { new List<long> { 123, 456 }, 1, DateTime.Now },
                    new object[] { new List<long> { 789 }, 2, new DateTime(2021, 05, 01) }
                };
            }
        }

        public static IEnumerable<object[]> ExpectedUpdateWhitelistMemberBigStake
        {
            get
            {
                return new List<object[]>
                {
                    new object[] { new List<long> { 178, 546 }, 1 },
                    new object[] { new List<long> { 7863, 45696}, 2 }
                };
            }
        }
        
        public static AutoMailBigCreditMemberModel ExpectedGetBigCreditMemberForAutomail
        {
            get
            {
                return new AutoMailBigCreditMemberModel
                {
                    BigCreditMemberList = new List<AutomailBigCreditMemberEntity>(),
                    BigCreditMemberWithBigLossList = new List<AutomailBigCreditMemberEntity>()
                };
            }
        }

    }
}
