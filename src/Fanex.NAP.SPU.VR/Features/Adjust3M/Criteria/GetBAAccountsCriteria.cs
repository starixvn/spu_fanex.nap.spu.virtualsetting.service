namespace Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria
{
    using Fanex.Data.Repository;

    public class GetBAAccountsCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerVirtualRateSelectBA";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
