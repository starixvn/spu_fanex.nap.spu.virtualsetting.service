using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.VR.Features.BetLimit.Criteria
{
    public class VerifyCustomerBetLimitCriteria: CriteriaBase
    {
        public string ListCustID { get; set; }

        public override string GetSettingKey()
        {
            return "Task_BetLimit_CheckCustPerformance";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
