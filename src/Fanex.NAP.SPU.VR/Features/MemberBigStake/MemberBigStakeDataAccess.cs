using Fanex.Data.Repository;
using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.VR.Features.MemberBigStake
{
    public interface IMemberBigStakeDataAccess
    {
        MemberBigStakePerformanceModel GetMemberBigStakePerformance();

        void UpdateVRMemeberBigStakeFirstBetDate(string custIdList, int type, DateTime betDate);

        void InsertVRMemberBigStake();

        void UpdateVRWhitelistMemberBigStake(string custIdList, int type);

        AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail();
    }

    public class MemberBigStakeDataAccess: IMemberBigStakeDataAccess
    {
        private readonly IDynamicRepository _repository;

        public MemberBigStakeDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public AutoMailBigCreditMemberModel GetBigCreditMemberForAutomail()
        {
            throw new NotImplementedException();
        }

        public MemberBigStakePerformanceModel GetMemberBigStakePerformance()
        {
            throw new NotImplementedException();
        }

        public void InsertVRMemberBigStake()
        {
            throw new NotImplementedException();
        }

        public void UpdateVRMemeberBigStakeFirstBetDate(string custIdList, int type, DateTime betDate)
        {
            throw new NotImplementedException();
        }

        public void UpdateVRWhitelistMemberBigStake(string custIdList, int type)
        {
            throw new NotImplementedException();
        }
    }
}
