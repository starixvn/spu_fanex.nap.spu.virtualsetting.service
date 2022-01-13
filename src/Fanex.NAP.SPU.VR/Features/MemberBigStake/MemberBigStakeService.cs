namespace Fanex.NAP.SPU.VR.Features.MemberBigStake
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;    
    using System;
    using System.Collections.Generic;

    public interface IMemberBigStakeService
    {
        void InsertMemberBigStake(bool isTest);

        MemberBigStakePerformanceModel GetMemberBigStakePerformance(bool isTest);

        void UpdateMemberBigStakeFirstBetDate(IEnumerable<long> customerList, int type, DateTime betDate, bool isTest);

        void UpdateWhitelistMemberBigStake(IEnumerable<MemberBigStakePerformanceStatusModel> customerList, int type, bool isTest);

        AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail();
    }

    public class MemberBigStakeService : IMemberBigStakeService
    {
        private readonly IMemberBigStakeDataAccess memberBigStakeDataAccess;

        public MemberBigStakeService(IMemberBigStakeDataAccess memberBigStakeDataAccess)
        {
            this.memberBigStakeDataAccess = memberBigStakeDataAccess;
        }

        public MemberBigStakePerformanceModel GetMemberBigStakePerformance(bool isTest)
        {
            return this.memberBigStakeDataAccess.GetMemberBigStakePerformance(isTest); 
        }

        public void InsertMemberBigStake(bool isTest)
        {
            this.memberBigStakeDataAccess.InsertMemberBigStake(isTest);
        }

        public void UpdateMemberBigStakeFirstBetDate(IEnumerable<long> customerList, int type, DateTime betDate, bool isTest)
        {
            var custIdList = string.Join(",", customerList);

            this.memberBigStakeDataAccess.UpdateMemeberBigStakeFirstBetDate(custIdList, type, betDate, isTest);
        }

        public void UpdateWhitelistMemberBigStake(IEnumerable<MemberBigStakePerformanceStatusModel> customerList, int type, bool isTest)
        {
            var custXml = string.Empty;

            foreach (var item in customerList)
            {
                object[] objParams =
                {
                    item.CustID,
                    item.WL
                };

                custXml += Utilities.GenerateXmlForMemberBigStakeWhiteListUpd(objParams);
            }

            custXml = Utilities.GenerateXmlForVRRoot(custXml);

            this.memberBigStakeDataAccess.UpdateWhitelistMemberBigStake(custXml, type, isTest);
        }

        public AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail()
        {
            return this.memberBigStakeDataAccess.GetBigCreditMemberForAutomail();
        }

    }
}
