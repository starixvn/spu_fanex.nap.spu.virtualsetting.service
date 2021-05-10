using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.VR.Features.BetLimit.Criteria
{
    public class GetCustomerBetLimitCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "NAP_BLControl_GetPerformanceCheckList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
