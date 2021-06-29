namespace Fanex.NAP.SPU.VR.Features.Agent.Criteria
{
    using Fanex.Data.Repository;

    public class InsertNewAgentAdjustCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Acc_VR_AgentAutoAdjustment_Ins";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
