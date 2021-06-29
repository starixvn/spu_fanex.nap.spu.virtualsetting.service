namespace Fanex.NAP.SPU.VR.Features.Agent.Criteria
{
    using Fanex.Data.Repository;

    public class GetVRAgentUpdatedCriteria : CriteriaBase
    {
        public override string GetSettingKey()
           => "Acc_Rpt_Automail_AgentsGotVRUpdated";

        public override bool IsValid()
            => true;
    }
}
