namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Criteria
{
    using Fanex.Data.Repository;

    public class CheckMemberBigStakePerformanceCriteria : CriteriaBase
    {
        public bool IsTest { get; set; }

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
