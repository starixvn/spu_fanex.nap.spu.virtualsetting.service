namespace Fanex.NAP.SPU.VR.Features.BetLimit.Criteria
{
    using Fanex.Data.Repository;

    public class RemoveCustomerBetLimitCriteria: CriteriaBase
    {
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_BLControl_PerformanceChecking_RemoveBetLimit";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIdList);
        }
    }
}
