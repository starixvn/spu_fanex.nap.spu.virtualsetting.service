using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Criteria
{
    public class UpdateWhitelistVRMemberBigStakeCriteria: CriteriaBase
    {
        public string CustIdList { get; set; }

        public int Type { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_MemberBigStake_WhiteList_Upd";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
