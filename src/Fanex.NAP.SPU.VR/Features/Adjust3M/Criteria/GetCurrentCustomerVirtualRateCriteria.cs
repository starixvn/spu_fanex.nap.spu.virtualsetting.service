namespace Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria
{
    using Fanex.Data.Repository;

    public class GetCurrentCustomerVirtualRateCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_Report_CustMngt_GetCustomersVirtualRate";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
