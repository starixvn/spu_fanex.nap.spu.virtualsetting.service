namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateFirstBetDateCriteria : CriteriaBase
    {
        public string CustIDList { get; set; }

        public int Sporttype { get; set; }

        public DateTime BetDate { get; set; }

        public bool IsTest { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_VR_CustomerUpdFirstBetDate";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDList);
        }
    }
}
