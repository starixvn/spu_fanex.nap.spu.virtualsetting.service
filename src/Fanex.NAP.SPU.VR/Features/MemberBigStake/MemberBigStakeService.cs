namespace Fanex.NAP.SPU.VR.Features.MemberBigStake
{
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using System;
    using System.Collections.Generic;

    public interface IMemberBigStakeService
    {
        void InsertMemberBigStake();

        MemberBigStakePerformanceModel GetMemberBigStakePerformance();

        void UpdateMemberBigStakeFirstBetDate(IEnumerable<long> customerList, int type, DateTime betDate);

        void UpdateWhitelistMemberBigStake(IEnumerable<long> customerList, int type);

        AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail();
    }

    public class MemberBigStakeService : IMemberBigStakeService
    {
        private readonly IMemberBigStakeDataAccess memberBigStakeDataAccess;

        public MemberBigStakeService(IMemberBigStakeDataAccess memberBigStakeDataAccess)
        {
            this.memberBigStakeDataAccess = memberBigStakeDataAccess;
        }

        public MemberBigStakePerformanceModel GetMemberBigStakePerformance()
        {
            return this.memberBigStakeDataAccess.GetMemberBigStakePerformance(); 
        }

        public void InsertMemberBigStake()
        {
            this.memberBigStakeDataAccess.InsertMemberBigStake();
        }

        public void UpdateMemberBigStakeFirstBetDate(IEnumerable<long> customerList, int type, DateTime betDate)
        {
            var custIdList = string.Join(",", customerList);

            this.memberBigStakeDataAccess.UpdateMemeberBigStakeFirstBetDate(custIdList, type, betDate);
        }

        public void UpdateWhitelistMemberBigStake(IEnumerable<long> customerList, int type)
        {
            var custIdList = string.Join(",", customerList);

            this.memberBigStakeDataAccess.UpdateWhitelistMemberBigStake(custIdList, type);
        }

        public AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail()
        {
            return this.memberBigStakeDataAccess.GetBigCreditMemberForAutomail();
        }

    }
}
