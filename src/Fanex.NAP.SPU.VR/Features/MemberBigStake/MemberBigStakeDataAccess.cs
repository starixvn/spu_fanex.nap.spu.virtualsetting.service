namespace Fanex.NAP.SPU.VR.Features.MemberBigStake
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Criteria;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using Newtonsoft.Json;
    using System;

    public interface IMemberBigStakeDataAccess
    {
        MemberBigStakePerformanceModel GetMemberBigStakePerformance(bool isTest);

        void UpdateMemeberBigStakeFirstBetDate(string custIdList, int type, DateTime betDate, bool isTest);

        void InsertMemberBigStake(bool isTest);

        void UpdateWhitelistMemberBigStake(string custIdList, int type, bool isTest);

        AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail();
    }

    public class MemberBigStakeDataAccess: IMemberBigStakeDataAccess
    {
        private readonly IDynamicRepository _repository;

        public MemberBigStakeDataAccess()
        {
            _repository = new DynamicRepository();
        }       

        public MemberBigStakePerformanceModel GetMemberBigStakePerformance(bool isTest)
        {
            var criteria = new CheckMemberBigStakePerformanceCriteria { IsTest = isTest };

            var result = _repository.FetchMultiple<MemberBigStakePerformanceFirstDayEntity, MemberBigStakePerformanceStatusEntity>(criteria);
           
            return new MemberBigStakePerformanceModel
            {
                FirstBetDateList = result.FirstResultSet,
                StatusList = result.SecondResultSet
            };
        }

        public void InsertMemberBigStake(bool isTest)
        {
            var criteria = new InsertMemberBigStakeCriteria { IsTest = isTest };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateMemeberBigStakeFirstBetDate(string custIdList, int type, DateTime betDate, bool isTest)
        {
            var criteria = new UpdateFirstBetDateVRMemberBigStakeCriteria
            {
                CustIdList = custIdList,
                Type = type,
                BetDate = betDate,
                IsTest = isTest
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateWhitelistMemberBigStake(string custIdList, int type, bool isTest)
        {
            var criteria = new UpdateWhitelistVRMemberBigStakeCriteria
            {
                CustIdList = custIdList,
                Type = type,
                IsTest = isTest
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail()
        {
            var criteria = new AutoMailBigCreditMemberCriteria();

            var result = _repository.FetchMultiple<AutomailBigCreditMemberEntity, AutomailBigCreditMemberEntity>(criteria,"");

            return new AutoMailBigCreditMemberModel
            {
                BigCreditMemberList = result.FirstResultSet,
                BigCreditMemberWithBigLossList = result.SecondResultSet
            };
        }
    }
}
