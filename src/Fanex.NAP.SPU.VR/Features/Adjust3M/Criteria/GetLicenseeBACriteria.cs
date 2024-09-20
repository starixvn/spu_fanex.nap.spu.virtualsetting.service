namespace Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria
{
    using Fanex.Data.Repository;

    public class GetLicenseeBACriteria : CriteriaBase
    {
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
