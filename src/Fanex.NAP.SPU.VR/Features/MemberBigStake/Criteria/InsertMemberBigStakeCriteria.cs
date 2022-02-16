namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Criteria
{
    using Fanex.Data.Repository;

    public class InsertMemberBigStakeCriteria : CriteriaBase
    {
        public bool IsTest { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_MemberBigStake_Ins";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
