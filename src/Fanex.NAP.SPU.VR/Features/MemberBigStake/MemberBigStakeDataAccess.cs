namespace Fanex.NAP.SPU.VR.Features.MemberBigStake
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Criteria;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using System;

    public interface IMemberBigStakeDataAccess
    {
        MemberBigStakePerformanceModel GetMemberBigStakePerformance();

        void UpdateMemeberBigStakeFirstBetDate(string custIdList, int type, DateTime betDate);

        void InsertMemberBigStake();

        void UpdateWhitelistMemberBigStake(string custIdList, int type);

        AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail();
    }

    public class MemberBigStakeDataAccess: IMemberBigStakeDataAccess
    {
        private readonly IDynamicRepository _repository;

        public MemberBigStakeDataAccess()
        {
            _repository = new DynamicRepository();
        }       

        public MemberBigStakePerformanceModel GetMemberBigStakePerformance()
        {
            var criteria = new CheckMemberBigStakePerformanceCriteria();

            var result = _repository.FetchMultiple<MemberBigStakePerformanceFirstDayEntity, MemberBigStakePerformanceStatusEntity>(criteria);

            return new MemberBigStakePerformanceModel
            {
                FirstBetDateList = result.firstResultSet,
                StatusList = result.secondResultSet
            };
        }

        public void InsertMemberBigStake()
        {
            var criteria = new InsertMemberBigStakeCriteria();

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateMemeberBigStakeFirstBetDate(string custIdList, int type, DateTime betDate)
        {
            var criteria = new UpdateFirstBetDateVRMemberBigStakeCriteria
            {
                CustIdList = custIdList,
                Type = type,
                BetDate = betDate
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateWhitelistMemberBigStake(string custIdList, int type)
        {
            var criteria = new UpdateWhitelistVRMemberBigStakeCriteria
            {
                CustIdList = custIdList,
                Type = type,
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail()
        {
            var criteria = new AutoMailBigCreditMemberCriteria();

            var result = _repository.FetchMultiple<AutomailBigCreditMemberEntity, AutomailBigCreditMemberEntity>(criteria);

            return new AutoMailBigCreditMemberModel
            {
                BigCreditMemberList = result.firstResultSet,
                BigCreditMemberWithBigLossList = result.secondResultSet
            };
        }
    }
}
