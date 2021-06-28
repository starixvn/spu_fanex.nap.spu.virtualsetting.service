using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Criteria
{
    public class AutoMailBigCreditMemberCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Acc_Rpt_Automail_BigCreditMembers";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
