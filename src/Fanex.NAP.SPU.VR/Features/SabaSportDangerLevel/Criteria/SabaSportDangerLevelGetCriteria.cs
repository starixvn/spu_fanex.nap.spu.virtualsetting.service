namespace Fanex.NAP.SPU.CTS.CC.Features.SabaSportDangerLevel
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SabaSportDangerLevelGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "IncludeUUS")]
        public int IncludeUUS { get; set; }

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