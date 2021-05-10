using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.VR.Features.BetLimit.Criteria
{
    public class UpdateCustomerBetLimitCriteria: CriteriaBase
    {
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_BLControl_PerformanceChecking_ReduceBetLimit";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIdList);
        }
    }
}
