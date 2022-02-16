using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Criteria
{
    public class UpdateFirstBetDateVRMemberBigStakeCriteria : CriteriaBase
    {
        public string CustIdList { get; set; }

        public int Type { get; set; }

        public DateTime BetDate { get; set; }

        public bool IsTest { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_MemberBigStake_FirstBetDate_Upd";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
