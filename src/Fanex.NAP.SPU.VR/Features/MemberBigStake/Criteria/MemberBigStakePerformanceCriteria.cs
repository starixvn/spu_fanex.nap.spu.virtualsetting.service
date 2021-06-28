using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Criteria
{
    public class MemberBigStakePerformanceCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Task_VRSetting_CheckMemberBigStakePerformance";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
