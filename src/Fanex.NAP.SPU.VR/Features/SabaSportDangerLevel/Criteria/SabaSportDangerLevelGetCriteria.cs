namespace Fanex.NAP.SPU.CTS.CC.Features.SabaSportDangerLevel
{
    using Fanex.Data.Repository;

    public class SabaSportDangerLevelGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Acc_Rpt_SABASportsDangerLevel_GetData";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}